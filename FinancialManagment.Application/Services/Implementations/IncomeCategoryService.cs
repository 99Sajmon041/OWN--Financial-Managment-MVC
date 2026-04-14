using AutoMapper;
using FinancialManagment.Application.Exceptions;
using FinancialManagment.Application.Models.IncomeCategory;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Application.UserIdentity;
using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Shared.Grid.Filtering;
using FinancialManagment.Shared.Grid.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinancialManagment.Application.Services.Implementations;

public sealed class IncomeCategoryService(
    ILogger<IncomeCategoryService> logger,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IMapper mapper) : IIncomeCategoryService
{
    public async Task<PagedResult<IncomeCategoryViewModel>> GetAllAsync(GridRequest gridRequest, CancellationToken ct)
    {
        gridRequest.Normalize();

        var userId = currentUser.ValidatedUserId;

        IQueryable<IncomeCategory> query = unitOfWork.IncomeCategoryRepository.GetQueryable(userId);

        query = query.ApplyFilters(gridRequest.Filters);

        int totalItems = await query.CountAsync(ct);

        var pager = new Pager(totalItems, gridRequest.Page, gridRequest.PageSize);

        query = query.ApplySorting(gridRequest.SortOrder);
        query = query.ApplyPaging(pager);

        List<IncomeCategory> incomeCategories = await query.ToListAsync(ct);

        IReadOnlyList<IncomeCategoryViewModel> items = mapper.Map<List<IncomeCategoryViewModel>>(incomeCategories);

        logger.LogInformation("User with ID: {UserId} retrieved income categories grid. Total items after filtering: {TotalItemsCount}. " +
            "Current page: {CurrentPage}. Page size: {PageSize}.",
            userId,
            totalItems,
            gridRequest.Page,
            gridRequest.PageSize);

        return new PagedResult<IncomeCategoryViewModel>
        {
            Items = items,
            Pager = pager,
            GridRequest = gridRequest,
            FilterModelType = typeof(IncomeCategory)
        };
    }

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

    public async Task ChangeStatusAsync(int id, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        var incomeCategory = await unitOfWork.IncomeCategoryRepository.GetByIdAsync(id, userId, ct);
        if (incomeCategory is null)
        {
            logger.LogWarning("User with ID: {UserId} attempted to change status of income category with ID: {IncomeCategoryId}, but it was not found.", userId, id);
            throw new NotFoundException($"Kategorie s ID: {id} nebyla nalezena.");
        }

        string action;

        if (incomeCategory.IsActive)
        {
            incomeCategory.IsActive = false;
            action = "deactivated";
        }
        else
        {
            incomeCategory.IsActive = true;
            action = "activated";
        }

        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User with ID: {UserId} {Phrase} income category with ID: {IncomeCategoryId}.", userId, action, id);
    }

    public async Task<IncomeCategoryUpsertViewModel?> GetByIdAsync(int id, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        var incomeCategory = await unitOfWork.IncomeCategoryRepository.GetByIdAsync(id, userId, ct);
        if (incomeCategory is null)
        {
            logger.LogWarning("User with ID: {UserId} attempted to get income category with ID: {IncomeCategoryId}, but it was not found.", userId, id);
            throw new NotFoundException($"Kategorie s ID: {id} nebyla nalezena.");
        }

        return mapper.Map<IncomeCategoryUpsertViewModel>(incomeCategory);
    }
}