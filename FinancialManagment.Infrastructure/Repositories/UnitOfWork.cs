using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Infrastructure.Database;

namespace FinancialManagment.Infrastructure.Repositories;

public sealed class UnitOfWork(FinancialManagmentDbContext context) : IUnitOfWork
{
    private IExpenseCategoryRepository? expenseCategoryRepository;
    private IExpenseRepository? expenseRepository;
    private IIncomeCategoryRepository? incomeCategoryRepository;
    private IIncomeRepository? incomeRepository;
    private IHouseholdMemberRepository? householdMemberRepository;

    public IExpenseCategoryRepository ExpenseCategoryRepository => expenseCategoryRepository ??= new ExpenseCategoryRepository(context);
    public IExpenseRepository ExpenseRepository => expenseRepository ??= new ExpenseRepository(context);
    public IIncomeCategoryRepository IncomeCategoryRepository => incomeCategoryRepository ??= new IncomeCategoryRepository(context);
    public IIncomeRepository IncomeRepository => incomeRepository ??= new IncomeRepository(context);
    public IHouseholdMemberRepository HouseholdMemberRepository => householdMemberRepository ??= new HouseholdMemberRepository(context);

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await context.SaveChangesAsync(ct);
    }
}
