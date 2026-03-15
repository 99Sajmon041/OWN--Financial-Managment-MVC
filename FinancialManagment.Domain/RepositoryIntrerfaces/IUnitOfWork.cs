namespace FinancialManagment.Domain.RepositoryIntrerfaces;

public interface IUnitOfWork
{
    IExpenseCategoryRepository ExpenseCategoryRepository { get; }
    IExpenseRepository ExpenseRepository { get; }
    IIncomeCategoryRepository IncomeCategoryRepository { get; }
    IIncomeRepository IncomeRepository { get; }
    IHouseholdMemberRepository HouseholdMemberRepository { get; }
    Task SaveChangesAsync(CancellationToken ct);
}
