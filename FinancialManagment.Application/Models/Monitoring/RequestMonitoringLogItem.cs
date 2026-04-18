using FinancialManagment.Shared.Attributes;
using System.ComponentModel.DataAnnotations;

namespace FinancialManagment.Application.Models.Monitoring;

[FilterGroup("Monitoring log")]
public sealed class RequestMonitoringLogItem
{
    [NotFilterable]
    [Display(Name = "Datum")]
    public DateTime Timestamp { get; set; }

    [NotFilterable]
    [Display(Name = "HTTP metoda")]
    public string HttpMethod { get; set; } = string.Empty;

    [NotFilterable]
    [Display(Name = "Cesta (URL)")]
    public string Path { get; set; } = string.Empty;

    [NotFilterable]
    [Display(Name = "Kontroler")]
    public string Controller { get; set; } = string.Empty;

    [NotFilterable]
    [Display(Name = "Akce")]
    public string Action { get; set; } = string.Empty;

    [FilterOrder(3)]
    [FilterLabel("Délka trvání requestu (ms)")]
    [Display(Name = "Doba požadavku (ms)")]
    public long DurationMs { get; set; }

    [FilterOrder(1)]
    [FilterLabel("Doba práce procesoru (ms)")]
    [Display(Name = "Doba práce procesoru (ms)")]
    public long CpuTimeUsedMs { get; set; }

    [FilterOrder(2)]
    [FilterLabel("Využití paměti (MB)")]
    [Display(Name = "Využití paměti (MB)")]
    public double MemoryUsageMb { get; set; }

    [NotFilterable]
    [Display(Name = "Status kód")]
    public string StatusCode { get; set; } = string.Empty;
}
