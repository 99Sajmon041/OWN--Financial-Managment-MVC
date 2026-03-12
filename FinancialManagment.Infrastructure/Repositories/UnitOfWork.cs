using FinancialManagment.Domain.RepositoryIntrerfaces;
using FinancialManagment.Infrastructure.Database;

namespace FinancialManagment.Infrastructure.Repositories;

public sealed class UnitOfWork(FinancialManagmentDbContext context) : IUnitOfWork
{
    private IExpenseCategoryRepository? expenseCategoryRepository;
    private IExpenseRepository? expenseRepository;
    private IIncomeCategory? incomeCategory;
    private IIncomeRepository? incomeRepository;

    public IExpenseCategoryRepository ExpenseCategoryRepository => expenseCategoryRepository ??= new ExpenseCategoryRepository(context);
    public IExpenseRepository ExpenseRepository => expenseRepository ??= new ExpenseRepository(context);
    public IIncomeCategory IncomeCategory => incomeCategory ??= new IncomeCategory(context);
    public IIncomeRepository IncomeRepository => incomeRepository ??= new IncomeRepository(context);

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await context.SaveChangesAsync(ct);
    }
}
