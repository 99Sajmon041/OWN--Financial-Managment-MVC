using FinancialManagment.Domain.RepositoryIntrerfaces;
using FinancialManagment.Infrastructure.Database;

namespace FinancialManagment.Infrastructure.Repositories;

public sealed class IncomeCategory(FinancialManagmentDbContext context) : IIncomeCategory
{

}
