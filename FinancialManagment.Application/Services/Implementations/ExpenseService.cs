using AutoMapper;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Domain.RepositoryInterfaces;
using Microsoft.Extensions.Logging;

namespace FinancialManagment.Application.Services.Implementations;

public sealed class ExpenseService(
    ILogger<ExpenseService> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IExpenseService
{

}
