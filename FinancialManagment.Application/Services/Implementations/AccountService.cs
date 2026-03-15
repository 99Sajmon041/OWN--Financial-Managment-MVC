using FinancialManagment.Application.Exceptions;
using FinancialManagment.Application.Models.Account;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryIntrerfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace FinancialManagment.Application.Services.Implementations;

public sealed class AccountService(
    ILogger<AccountService> logger,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IUnitOfWork unitOfWork) : IAccountService
{
    public async Task RegisterAsync(RegisterViewModel model, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var trimmedEmail = model.Email.Trim();

        var newUser = new ApplicationUser
        {
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            Email = trimmedEmail,
            UserName = trimmedEmail
        };

        var usersEmailExists = await userManager.FindByEmailAsync(trimmedEmail);
        if (usersEmailExists is not null)
        {
            logger.LogWarning("Creating user with e-mail: {Email} failed. User with e-mail already exists.", newUser.Email);
            throw new ConflictException("Účet se nepodařilo vytvořit. Tento e-mail patří jinému uživateli, použijte jiný.");
        }

        var registerResult = await userManager.CreateAsync(newUser, model.Password);
        if(!registerResult.Succeeded)
        {
            var errorMessage = string.Join(", ", registerResult.Errors.Select(e => e.Description));

            logger.LogWarning("Creating user with e-mail: {Email} failed. Message: {Error}.", newUser.Email, errorMessage);
            throw new DomainException("Účet se nepodařilo vytvořit. Kontaktujte administrátora webu.");
        }

        var houseHoldMember = new HouseholdMember
        {
            ApplicationUserId = newUser.Id,
            Nickname = model.Nickname.Trim()
        };

        try
        {
            unitOfWork.HouseholdMemberRepository.Add(houseHoldMember);
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch(Exception ex)
        {
            await userManager.DeleteAsync(newUser);
            logger.LogError(ex, "Creating default household member for user with e-mail: {Email} failed. User account was rolled back.", newUser.Email);
            throw new DomainException("Nepodařilo se vytvořit uživatele. Kontaktujte prosím administrátora webu.");
        }

        logger.LogInformation("User with e-mail: {Email} was created successfully.", newUser.Email);
    }

    public async Task LoginAsync(LoginViewModel model, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var trimmedEmail = model.Email.Trim();

        var user = await userManager.FindByEmailAsync(trimmedEmail);
        if (user is null)
        {
            logger.LogWarning("User with e-mail: {Email} does not exists.", trimmedEmail);
            throw new DomainException("Neplatné přihlašovací údaje.");
        }

        var loginResult = await signInManager.PasswordSignInAsync(user, model.Password, true, true);
        if (!loginResult.Succeeded)
        {
            logger.LogWarning("User with e-mail: {Email} entered an incorrect password. Login failed.", trimmedEmail);
            throw new DomainException("Neplatné přihlašovací údaje.");
        }

        logger.LogInformation("User with e-mail: {Email} logged in successfully.", trimmedEmail);
    }

    public async Task LogoutAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        await signInManager.SignOutAsync();
    }
}
