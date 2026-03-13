using FinancialManagment.Application.Exceptions;
using FinancialManagment.Application.Models.IncomeCategory;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Application.UserIdentity;
using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryIntrerfaces;
using Microsoft.Extensions.Logging;

namespace FinancialManagment.Application.Services.Implementations;

public sealed class IncomeCategoryService(
    ILogger<IncomeCategoryService> logger,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser) : IIncomeCategoryService
{
    public async Task AddAsync(IncomeCategoryUpsertViewModel model, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;
        var name = model.Name.Trim();

        var existByName = await unitOfWork.IncomeCategoryRepository.ExistsByNameAsync(name, userId, ct);
        if (existByName)
        {
            logger.LogWarning("User with ID: {UserId} tries to add incoming category with name, that already exists.", userId);
            throw new ConflictException($"Kategorie s názvem: {name} již existuje.");
        }

        var incomeCategory = new IncomeCategory
        {
            ApplicationUserId = userId,
            Name = model.Name,
            IsActive = true
        };

        unitOfWork.IncomeCategoryRepository.Add(incomeCategory);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User with ID: {UserId} added a new income category with name: {IncomeCategoryName}.", userId, model.Name);
    }

    public async Task UpdateAsync(int id, IncomeCategoryUpsertViewModel model, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        if (id != model.Id)
        {
            logger.LogWarning("User with ID: {UserId} attempted to update income category, but route ID does not match model ID.", userId);
            throw new DomainException("ID parametr se neshoduje.");
        }

        var incomeCategory = await unitOfWork.IncomeCategoryRepository.GetByIdAsync(id, userId, ct);
        if (incomeCategory is null)
        {
            logger.LogWarning("User with ID: {UserId} attempted to update income category with ID: {IncomeCategoryId}, but it was not found.", userId, id);
            throw new DomainException($"Kategorie s ID: {id} nebyla nalezena.");
        }

        var existsByName = await unitOfWork.IncomeCategoryRepository.ExistsByNameWithDifferentIdAsync(model.Name, id, userId, ct);

        if (existsByName)
        {
            logger.LogWarning("User with ID: {UserId} attempted to update income category with name: {IncomeCategoryName}, but that name already exists.", userId, model.Name);
            throw new ConflictException($"Kategorie s názvem: {model.Name} již existuje.");
        }

        incomeCategory.Name = model.Name;
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User with ID: {UserId} updated income category with ID: {IncomeCategoryId}.", userId, incomeCategory.Id);
    }
}