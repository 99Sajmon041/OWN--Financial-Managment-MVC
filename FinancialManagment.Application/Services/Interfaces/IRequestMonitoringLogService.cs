using FinancialManagment.Application.Models.Monitoring;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Shared.Grid.Paging;
using System.Reflection;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IRequestMonitoringLogService
{
    Task WriteLogAsync(RequestMonitoringLogItem item, CancellationToken ct);
    Task<PagedResult<RequestMonitoringLogItem>> ReadLogsByDateAsync(GridRequest gridRequest, DateOnly date, Assembly assembly, CancellationToken ct);
    Task<List<RequestMonitoringLogItem>> GetAllReadLogsByDateAsync(DateOnly date, CancellationToken ct);
}
