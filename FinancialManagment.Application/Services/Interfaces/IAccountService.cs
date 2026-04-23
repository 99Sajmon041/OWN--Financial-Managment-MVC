using FinancialManagment.Application.Models.Account;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IAccountService
{
    Task RegisterAsync(RegisterViewModel model, CancellationToken ct);
    Task LoginAsync(LoginViewModel model, CancellationToken ct);
    Task LogoutAsync(CancellationToken ct);
    Task ChangePasswordAsync(ChangePasswordViewModel model, CancellationToken ct);
}