using FinancialManagment.Application.Models.Monitoring;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Application.UserIdentity;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Shared.Grid.Filtering;
using FinancialManagment.Shared.Grid.Paging;
using FinancialManagment.Shared.Utilities;
using System.Globalization;
using System.Reflection;

namespace FinancialManagment.Web.LoggingService;

public sealed class RequestMonitoringLogService(
    IWebHostEnvironment environment,
    ICurrentUser currentUser,
    ILogger<RequestMonitoringLogService> logger) : IRequestMonitoringLogService
{
    private readonly string logsDirectoryPath = Path.Combine(environment.ContentRootPath, "LogPowerUsage", "RequestMonitoring");

    public async Task WriteLogAsync(RequestMonitoringLogItem item, CancellationToken ct)
    {
        Directory.CreateDirectory(logsDirectoryPath);

        string filePath = GetFilePath(item.Timestamp);

        string logLine = CreateLogLine(item);

        await File.AppendAllTextAsync(filePath, logLine + Environment.NewLine, ct);
    }

    public async Task<PagedResult<RequestMonitoringLogItem>> ReadLogsByDateAsync(
        GridRequest gridRequest, 
        DateOnly date, 
        Assembly assembly,
        CancellationToken ct)
    {
        string filePath = GetFilePath(date);

        if (!File.Exists(filePath))
        {
            var logPager = new Pager(0, gridRequest.Page, gridRequest.PageSize);

            return new PagedResult<RequestMonitoringLogItem>
            {
                Items = [],
                Pager = logPager,
                GridRequest = gridRequest,
                FilterModelType = typeof(RequestMonitoringLogItem),
                CustomFilters = []
            };
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

        var customFilters = new List<FilterFieldDefinition>();

        var statusCodes = new FilterFieldDefinition
        {
            PropertyName = "StatusCode",
            PropertyPath = "StatusCode",
            Label = "Status kód",
            PropertyType = typeof(string),
            UnderlyingType = typeof(string),
            InputType = FilterInputType.Select,
            AllowedOperators =
            [
                FilterOperator.None,
                FilterOperator.Equal,
                FilterOperator.NotEqual
            ],
            SelectedOperator = FilterHelper.GetSelectedOperator(gridRequest.Filters, "StatusCode"),
            Value = FilterHelper.GetSelectedValue(gridRequest.Filters, "StatusCode"),
            Order = 4,
            GroupName = "Monitoring log",
            Options = OptionsBuilder.GetStatusCodes()
        };

        var httpMethods = new FilterFieldDefinition
        {
            PropertyName = "HttpMethod",
            PropertyPath = "HttpMethod",
            Label = "HTTP Methoda",
            PropertyType = typeof(string),
            UnderlyingType = typeof(string),
            InputType = FilterInputType.Select,
            AllowedOperators =
            [
                FilterOperator.None,
                FilterOperator.Equal,
                FilterOperator.NotEqual
            ],
            SelectedOperator = FilterHelper.GetSelectedOperator(gridRequest.Filters, "HttpMethod"),
            Value = FilterHelper.GetSelectedValue(gridRequest.Filters, "HttpMethod"),
            Order = 5,
            GroupName = "Monitoring log",
            Options = OptionsBuilder.GetMethods()
        };

        var paths = new FilterFieldDefinition
        {
            PropertyName = "Path",
            PropertyPath = "Path",
            Label = "Cesta (URL)",
            PropertyType = typeof(string),
            UnderlyingType = typeof(string),
            InputType = FilterInputType.Select,
            AllowedOperators =
            [
                FilterOperator.None,
                FilterOperator.Equal,
                FilterOperator.NotEqual
            ],
            SelectedOperator = FilterHelper.GetSelectedOperator(gridRequest.Filters, "Path"),
            Value = FilterHelper.GetSelectedValue(gridRequest.Filters, "Path"),
            Order = 6,
            GroupName = "Monitoring log",
            Options = OptionsBuilder.GetEndpointPaths(assembly)
        };


        customFilters.Add(statusCodes);
        customFilters.Add(httpMethods);
        customFilters.Add(paths);

        queryResult = queryResult.ApplyFilters(gridRequest.Filters);

        int totalItems = queryResult.Count();

        var pager = new Pager(totalItems, gridRequest.Page, gridRequest.PageSize);

        queryResult = queryResult.OrderBy(x => x.Timestamp);
        queryResult = queryResult.ApplyPaging(pager);

        result = queryResult.ToList();

        var userId = currentUser.ValidatedUserId;

        logger.LogInformation("User with ID: {UserId} checks application usage statistics.", userId);

        return new PagedResult<RequestMonitoringLogItem>
        {
            Items = result,
            Pager = pager,
            GridRequest = gridRequest,
            FilterModelType = typeof(RequestMonitoringLogItem),
            CustomFilters = customFilters
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

    private static string CreateLogLine(RequestMonitoringLogItem item)
    {
        return string.Join("|",
            item.Timestamp.ToString("O", CultureInfo.InvariantCulture),
            item.HttpMethod,
            item.Path,
            item.Controller,
            item.Action,
            item.DurationMs,
            item.CpuTimeUsedMs.ToString(CultureInfo.InvariantCulture),
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
        bool parsedCpuTime = long.TryParse(parts[6], out long cpuTimeUsedMs);
        bool parsedMemory = double.TryParse(parts[7], CultureInfo.InvariantCulture, out double memoryUsageMb);
        bool parsedStatusCode = int.TryParse(parts[8], out int statusCode);

        if (!parsedTimestamp || !parsedDuration || !parsedCpuTime || !parsedMemory || !parsedStatusCode)
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
            CpuTimeUsedMs = cpuTimeUsedMs,
            MemoryUsageMb = memoryUsageMb,
            StatusCode = statusCode.ToString()
        };
    }
}
