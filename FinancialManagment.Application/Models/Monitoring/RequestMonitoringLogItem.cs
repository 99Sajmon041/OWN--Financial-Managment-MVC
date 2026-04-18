namespace FinancialManagment.Application.Models.Monitoring;

public sealed class RequestMonitoringLogItem
{
    public DateTime Timestamp { get; set; }
    public string HttpMethod { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public long DurationMs { get; set; }
    public double CpuUsagePercent { get; set; }
    public double MemoryUsageMb { get; set; }
    public int StatusCode { get; set; }
}
