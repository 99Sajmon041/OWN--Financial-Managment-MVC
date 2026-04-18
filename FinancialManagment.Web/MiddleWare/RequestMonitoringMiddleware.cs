using FinancialManagment.Application.Models.Monitoring;
using FinancialManagment.Application.Services.Interfaces;
using System.Diagnostics;

namespace FinancialManagment.Web.MiddleWare;

public sealed class RequestMonitoringMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IRequestMonitoringLogService requestMonitoringLogService)
    {
        Process currentProcess = Process.GetCurrentProcess();

        DateTime timestamp = DateTime.UtcNow;
        TimeSpan cpuBefore = currentProcess.TotalProcessorTime;

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();

            currentProcess.Refresh();

            TimeSpan cpuAfter = currentProcess.TotalProcessorTime;
            long memoryAfterBytes = currentProcess.WorkingSet64;

            var logItem = new RequestMonitoringLogItem
            {
                Timestamp = timestamp,
                HttpMethod = context.Request.Method,
                Path = context.Request.Path.ToString(),
                Controller = GetControllerName(context),
                Action = GetActionName(context),
                DurationMs = stopwatch.ElapsedMilliseconds,
                CpuTimeUsedMs = CalculateCpuTimeUsedMs(cpuBefore, cpuAfter),
                MemoryUsageMb = ConvertBytesToMb(memoryAfterBytes),
                StatusCode = context.Response.StatusCode.ToString()
            };

            await requestMonitoringLogService.WriteLogAsync(logItem, context.RequestAborted);
        }
    }

    private static string GetControllerName(HttpContext context)
    {
        object? controller = context.GetRouteValue("controller");
        return controller?.ToString() ?? "-";
    }

    private static string GetActionName(HttpContext context)
    {
        object? action = context.GetRouteValue("action");
        return action?.ToString() ?? "-";
    }

    private static double ConvertBytesToMb(long bytes)
    {
        return Math.Round(bytes / 1024d / 1024d, 2);
    }

    private static long CalculateCpuTimeUsedMs(TimeSpan cpuBefore, TimeSpan cpuAfter)
    {
        double cpuUsedMs = (cpuAfter - cpuBefore).TotalMilliseconds;

        if (cpuUsedMs <= 0)
        {
            return 0;
        }

        return (long)Math.Round(cpuUsedMs, 0);
    }
}