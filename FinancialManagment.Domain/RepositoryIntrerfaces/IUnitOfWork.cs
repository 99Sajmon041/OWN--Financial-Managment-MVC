namespace FinancialManagment.Domain.RepositoryIntrerfaces;

public interface IUnitOfWork
{
    IExpenseCategoryRepository ExpenseCategoryRepository { get; }
    IExpenseRepository ExpenseRepository { get; }
    IIncomeCategory IncomeCategory { get; }
    IIncomeRepository IncomeRepository { get; }
    Task SaveChangesAsync(CancellationToken ct);
}
