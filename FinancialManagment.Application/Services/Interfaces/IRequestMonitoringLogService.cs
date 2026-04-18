using FinancialManagment.Application.Models.Monitoring;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Shared.Grid.Paging;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IRequestMonitoringLogService
{
    Task WriteLogAsync(RequestMonitoringLogItem item, CancellationToken ct);
    Task<PagedResult<RequestMonitoringLogItem>> ReadLogsByDateAsync(GridRequest gridRequest, DateOnly date, CancellationToken ct);
}
