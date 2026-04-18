using FinancialManagment.Application.Models.Monitoring;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Shared.Grid.Paging;
using System.Globalization;

namespace FinancialManagment.Web.LoggingService;

public sealed class RequestMonitoringLogService : IRequestMonitoringLogService
{
    private readonly string logsDirectoryPath;
    private readonly IWebHostEnvironment _environment;

    public RequestMonitoringLogService(IWebHostEnvironment environment)
    {
        _environment = environment;
        logsDirectoryPath = Path.Combine(_environment.ContentRootPath, "LogPowerUsage", "RequestMonitoring");
    }

    public async Task WriteLogAsync(RequestMonitoringLogItem item, CancellationToken ct)
    {
        Directory.CreateDirectory(logsDirectoryPath);

        string filePath = GetFilePath(item.Timestamp);

        string logLine = CreateLogLine(item);

        await File.AppendAllTextAsync(filePath, logLine + Environment.NewLine, ct);
    }

    public async Task<PagedResult<RequestMonitoringLogItem>> ReadLogsByDateAsync(GridRequest gridRequest, DateOnly date, CancellationToken ct)
    {
        string filePath = GetFilePath(date);

        if (!File.Exists(filePath))
        {
            return new PagedResult<RequestMonitoringLogItem> { };
        }

        string[] lines = await File.ReadAllLinesAsync(filePath, ct);

        List<RequestMonitoringLogItem> result = [];

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            RequestMonitoringLogItem? parsedItem = ParseLogLine(line);

            if (parsedItem is not null)
            {
                result.Add(parsedItem);
            }
        }

        var queryResult = result.AsQueryable();

        int totalItems = result.Count;

        var pager = new Pager(totalItems, gridRequest.Page, gridRequest.PageSize);

        queryResult = queryResult.ApplyPaging(pager);
        queryResult = queryResult.OrderBy(x => x.Timestamp);

        result = queryResult.ToList();

        return new PagedResult<RequestMonitoringLogItem>
        {
            Items = result,
            Pager = pager,
            GridRequest = gridRequest,
            FilterModelType = typeof(RequestMonitoringLogItem)
        };
    }

    private string GetFilePath(DateTime timestamp)
    {
        return Path.Combine(logsDirectoryPath, $"request-monitoring-{timestamp:yyyy-MM-dd}.log");
    }

    private string GetFilePath(DateOnly date)
    {
        return Path.Combine(logsDirectoryPath, $"request-monitoring-{date:yyyy-MM-dd}.log");
    }

    private string CreateLogLine(RequestMonitoringLogItem item)
    {
        return string.Join("|",
            item.Timestamp.ToString("O", CultureInfo.InvariantCulture),
            item.HttpMethod,
            item.Path,
            item.Controller,
            item.Action,
            item.DurationMs,
            item.CpuUsagePercent,
            item.CpuUsagePercent.ToString(CultureInfo.InvariantCulture),
            item.MemoryUsageMb.ToString(CultureInfo.InvariantCulture),
            item.StatusCode);
    }

    private RequestMonitoringLogItem? ParseLogLine(string line)
    {
        string[] parts = line.Split('|');

        if (parts.Length != 9)
        {
            return null;
        }

        bool parsedTimestamp = DateTime.TryParse(
            parts[0],
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out DateTime timestamp);

        bool parsedDuration = long.TryParse(parts[5], out long durationMs);
        bool parsedCpu = double.TryParse(parts[6], CultureInfo.InvariantCulture, out double cpuUsagePercent);
        bool parsedMemory = double.TryParse(parts[7], CultureInfo.InvariantCulture, out double memoryUsageMb);
        bool parsedStatusCode = int.TryParse(parts[8], out int statusCode);

        if (!parsedTimestamp || !parsedDuration || !parsedCpu || !parsedMemory || !parsedStatusCode)
        {
            return null;
        }

        return new RequestMonitoringLogItem
        {
            Timestamp = timestamp,
            HttpMethod = parts[1],
            Path = parts[2],
            Controller = parts[3],
            Action = parts[4],
            DurationMs = durationMs,
            CpuUsagePercent = cpuUsagePercent,
            MemoryUsageMb = memoryUsageMb,
            StatusCode = statusCode
        };
    }
}
