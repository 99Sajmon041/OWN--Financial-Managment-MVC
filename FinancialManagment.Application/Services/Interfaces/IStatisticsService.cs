using FinancialManagment.Application.FilterModels;
using FinancialManagment.Application.Models.Statistics;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IStatisticsService
{
    Task<StatisticsViewModel> GetStatisticsAsync(StatisticsFilterModel model, CancellationToken ct);
}
