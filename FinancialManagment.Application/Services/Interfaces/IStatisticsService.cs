using FinancialManagment.Application.FilterModels;
using FinancialManagment.Application.Models.Statistics;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IStatisticsService
{
    Task<StatisticsJsViewModel> GetJsStatisticsAsync(StatisticsJsFilterModel model, CancellationToken ct);
}
