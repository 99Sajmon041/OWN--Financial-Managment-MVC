using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Infrastructure.Database;

namespace FinancialManagment.Infrastructure.Repositories;

public sealed class IncomeRepository(FinancialManagmentDbContext context) : IIncomeRepository
{

}
