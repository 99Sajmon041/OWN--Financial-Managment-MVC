using AutoMapper;
using FinancialManagment.Application.Exceptions;
using FinancialManagment.Application.Models.ExpenseCategory;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Application.UserIdentity;
using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Shared.Pagination;
using Microsoft.Extensions.Logging;

namespace FinancialManagment.Application.Services.Implementations;

public sealed class ExpenseCategoryService(
    ILogger<ExpenseCategoryService> logger,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IMapper mapper) : IExpenseCategoryService
{
    public async Task AddAsync(ExpenseCategoryUpsertViewModel model, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;
        var name = model.Name.Trim();

        var existByName = await unitOfWork.ExpenseCategoryRepository.ExistsByNameAsync(name, userId, ct);
        if (existByName)
        {
            logger.LogWarning("User with ID: {UserId} tries to add expense category with name, that already exists.", userId);
            throw new ConflictException($"Kategorie s názvem: {name} již existuje.");
        }

        var expenseCategory = new ExpenseCategory
        {
            ApplicationUserId = userId,
            Name = model.Name,
            IsActive = true
        };

        unitOfWork.ExpenseCategoryRepository.Add(expenseCategory);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User with ID: {UserId} added a new expense category with name: {ExpenseCategoryName}.", userId, model.Name);
    }

    public async Task ChangeStatusAsync(int id, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        var expenseCategory = await unitOfWork.ExpenseCategoryRepository.GetByIdAsync(id, userId, ct);
        if (expenseCategory is null)
        {
            logger.LogWarning("User with ID: {UserId} attempted to change status of expense category with ID: {ExpenseCategoryId}, but it was not found.", userId, id);
            throw new NotFoundException($"Kategorie s ID: {id} nebyla nalezena.");
        }

        string action;

        if (expenseCategory.IsActive)
        {
            expenseCategory.IsActive = false;
            action = "deactivated";
        }
        else
        {
            expenseCategory.IsActive = true;
            action = "activated";
        }

        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User with ID: {UserId} {Phrase} expense category with ID: {ExpenseCategoryId}.", userId, action, id);
    }

    public async Task<PagedResult<ExpenseCategoryViewModel>> GetAllAsync(PagedRequest pagedRequest, bool? isActive, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        pagedRequest = pagedRequest.Normalize();

        var (items, totalItemsCount) = await unitOfWork.ExpenseCategoryRepository.GetAllAsync(userId, isActive, pagedRequest, ct);

        var expenseCategories = mapper.Map<List<ExpenseCategoryViewModel>>(items);

        logger.LogInformation("User with ID: {UserId} retrieved expense categories list. Total items: {TotalItemsCount}.", userId, totalItemsCount);

        return new PagedResult<ExpenseCategoryViewModel>
        {
            Page = pagedRequest.Page,
            PageSize = pagedRequest.PageSize,
            Search = pagedRequest.Search,
            SortBy = pagedRequest.SortBy,
            Desc = pagedRequest.Desc,
            TotalItemsCount = totalItemsCount,
            Items = expenseCategories
        };
    }

    public async Task<ExpenseCategoryUpsertViewModel?> GetByIdAsync(int id, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        var expenseCategory = await unitOfWork.ExpenseCategoryRepository.GetByIdAsync(id, userId, ct);
        if (expenseCategory is null)
        {
            logger.LogWarning("User with ID: {UserId} attempted to get expense category with ID: {ExpenseCategoryId}, but it was not found.", userId, id);
            throw new NotFoundException($"Kategorie s ID: {id} nebyla nalezena.");
        }

        return mapper.Map<ExpenseCategoryUpsertViewModel>(expenseCategory);
    }

    public async Task UpdateAsync(int id, ExpenseCategoryUpsertViewModel model, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        if (id != model.Id)
        {
            logger.LogWarning("User with ID: {UserId} attempted to update expense category, but route ID does not match model ID.", userId);
            throw new DomainException("ID parametr se neshoduje.");
        }

        var expenseCategory = await unitOfWork.ExpenseCategoryRepository.GetByIdAsync(id, userId, ct);
        if (expenseCategory is null)
        {
            logger.LogWarning("User with ID: {UserId} attempted to update expense category with ID: {ExpenseCategoryId}, but it was not found.", userId, id);
            throw new DomainException($"Kategorie s ID: {id} nebyla nalezena.");
        }

        var existsByName = await unitOfWork.ExpenseCategoryRepository.ExistsByNameWithDifferentIdAsync(model.Name, id, userId, ct);

        if (existsByName)
        {
            logger.LogWarning("User with ID: {UserId} attempted to update expense category with name: {ExpenseCategoryName}, but that name already exists.", userId, model.Name);
            throw new ConflictException($"Kategorie s názvem: {model.Name} již existuje.");
        }

        expenseCategory.Name = model.Name;
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User with ID: {UserId} updated expense category with ID: {ExpenseCategoryId}.", userId, expenseCategory.Id);
    }
}
