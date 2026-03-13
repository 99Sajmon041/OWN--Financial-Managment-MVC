using FinancialManagment.Application.Services.Implementations;
using FinancialManagment.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialManagment.Application.Extensions;

public static class ApplicationServicesRegistrations
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IIncomeCategoryService, IncomeCategoryService>();

        return services;
    }
}
