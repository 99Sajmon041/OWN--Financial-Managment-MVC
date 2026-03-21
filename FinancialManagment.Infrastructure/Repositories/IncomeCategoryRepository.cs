using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Infrastructure.Database;

namespace FinancialManagment.Infrastructure.Repositories;

public sealed class IncomeCategoryRepository(FinancialManagementDbContext context) : BaseCategoryRepository<IncomeCategory>(context), IIncomeCategoryRepository { }