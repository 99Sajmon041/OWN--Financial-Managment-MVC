using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Infrastructure.Database;

namespace FinancialManagment.Infrastructure.Repositories;

public sealed class ExpenseCategoryRepository(FinancialManagmentDbContext context) : BaseCategoryRepository<ExpenseCategory>(context), IExpenseCategoryRepository { }
