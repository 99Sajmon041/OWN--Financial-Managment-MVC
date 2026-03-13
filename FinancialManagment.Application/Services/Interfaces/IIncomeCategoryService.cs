using FinancialManagment.Application.Models.IncomeCategory;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IIncomeCategoryService
{
    Task AddAsync(IncomeCategoryUpsertViewModel model, CancellationToken ct);
    Task UpdateAsync(int id, IncomeCategoryUpsertViewModel model, CancellationToken ct);
}