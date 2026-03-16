using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Infrastructure.Database;

namespace FinancialManagment.Infrastructure.Repositories;

public sealed class ExpenseRepository(FinancialManagmentDbContext context) : IExpenseRepository
{

}
