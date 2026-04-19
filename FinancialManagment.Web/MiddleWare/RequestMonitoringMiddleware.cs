using FinancialManagment.Application.Models.Monitoring;
using FinancialManagment.Application.Services.Interfaces;
using System.Diagnostics;

namespace FinancialManagment.Web.MiddleWare;

public sealed class RequestMonitoringMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IRequestMonitoringLogService requestMonitoringLogService)
    {
        Process currentProcess = Process.GetCurrentProcess();

        DateTime timestamp = DateTime.Now;
        TimeSpan cpuBefore = currentProcess.TotalProcessorTime;

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();

            string route = GetRoute(context);

            if (!string.IsNullOrWhiteSpace(route))
            {
                currentProcess.Refresh();

                TimeSpan cpuAfter = currentProcess.TotalProcessorTime;
                long memoryAfterBytes = currentProcess.WorkingSet64;

                string handledStatusCode = string.Empty;
                string handledExceptionType = string.Empty;

                if (context.Items.TryGetValue("HandledStatusCode", out object? handledStatusCodeValue))
                {
                    if (handledStatusCodeValue is int parsedStatusCode)
                    {
                        handledStatusCode = parsedStatusCode.ToString();
                    }
                }

                if (context.Items.TryGetValue("HandledExceptionType", out object? handledExceptionTypeValue))
                {
                    handledExceptionType = handledExceptionTypeValue?.ToString() ?? "-";
                }

                var logItem = new RequestMonitoringLogItem
                {
                    Timestamp = timestamp,
                    HttpMethod = context.Request.Method,
                    Path = route,
                    Controller = GetControllerName(context),
                    Action = GetActionName(context),
                    DurationMs = stopwatch.ElapsedMilliseconds,
                    CpuTimeUsedMs = CalculateCpuTimeUsedMs(cpuBefore, cpuAfter),
                    MemoryUsageMb = ConvertBytesToMb(memoryAfterBytes),
                    FinalStatusCode = context.Response.StatusCode.ToString(),
                    HandledStatusCode = handledStatusCode,
                    HandledExceptionType = handledExceptionType
                };

                await requestMonitoringLogService.WriteLogAsync(logItem, context.RequestAborted);
            }
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

    private static string GetRoute(HttpContext context)
    {
        object? controller = context.GetRouteValue("controller");
        object? action = context.GetRouteValue("action");

        if (controller is null || action is null)
        {
            return string.Empty;
        }

        return $"{controller}/{action}";
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