using AutoMapper;
using FinancialManagment.Application.Exceptions;
using FinancialManagment.Application.Models.Income;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Application.UserIdentity;
using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Shared.Grid.Filtering;
using FinancialManagment.Shared.Grid.Paging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinancialManagment.Application.Services.Implementations;

public sealed class IncomeService(
    ILogger<IncomeService> logger,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IMapper mapper) : IIncomeService
{
    public async Task<(PagedResult<IncomeViewModel>, decimal)> GetAllAsync(GridRequest gridRequest, CancellationToken ct)
    {
        gridRequest.Normalize();

        var userId = currentUser.ValidatedUserId;

        IQueryable<Income> query = unitOfWork.IncomeRepository.GetQueryable(userId);

        query = query.ApplyFilters(gridRequest.Filters);

        int totalItems = await query.CountAsync(ct);

        var pager = new Pager(totalItems, gridRequest.Page, gridRequest.PageSize);

        decimal totalAmount = await query
            .Select(x => (decimal?)x.Amount)
            .SumAsync(ct) ?? 0m;

        query = query.ApplySorting(gridRequest.SortOrder);
        query = query.ApplyPaging(pager); 

        List<Income> incomes = await query.ToListAsync(ct);

        IReadOnlyList<IncomeViewModel> items = mapper.Map<List<IncomeViewModel>>(incomes);

        var incomeCategories = await unitOfWork.IncomeCategoryRepository.GetAllCategoriesAsync(userId, ct);
        var householdMembers = await unitOfWork.HouseholdMemberRepository.GetAllAsync(userId, ct);
        var customFilters = new List<FilterFieldDefinition>();

        var categoryFilter = new FilterFieldDefinition
        {
            PropertyName = "IncomeCategory_Name",
            PropertyPath = "IncomeCategory.Name",
            Label = "Kategorie příjmu",
            PropertyType = typeof(string),
            UnderlyingType = typeof(string),
            InputType = FilterInputType.Select,
            AllowedOperators =
            [
                FilterOperator.None,
                FilterOperator.Equal,
                FilterOperator.NotEqual
            ],
            SelectedOperator = FilterHelper.GetSelectedOperator(gridRequest.Filters, "IncomeCategory_Name"),
            Value = FilterHelper.GetSelectedValue(gridRequest.Filters, "IncomeCategory_Name"),
            Order = 1,
            GroupName = "Kategorie příjmů",
            Options = incomeCategories.Select(x => new FilterOptionItem
            {
                Text = x.Name,
                Value = x.Name
            })
            .ToList()
        };

        var houseHoldmemberFilter = new FilterFieldDefinition
        {
            PropertyName = "HouseholdMember_Nickname",
            PropertyPath = "HouseholdMember.Nickname",
            Label = "Člen domácnosti",
            PropertyType = typeof(string),
            UnderlyingType = typeof(string),
            InputType = FilterInputType.Select,
            AllowedOperators =
            [
                FilterOperator.None,
                FilterOperator.Equal,
                FilterOperator.NotEqual
            ],
            SelectedOperator = FilterHelper.GetSelectedOperator(gridRequest.Filters, "HouseholdMember_Nickname"),
            Value = FilterHelper.GetSelectedValue(gridRequest.Filters, "HouseholdMember_Nickname"),
            Order = 2,
            GroupName = "Člen domácnosti",
            Options = householdMembers.Select(x => new FilterOptionItem
            {
                Text = x.Nickname,
                Value = x.Nickname
            })
            .ToList()
        };

        customFilters.Add(categoryFilter);
        customFilters.Add(houseHoldmemberFilter);

        logger.LogInformation("User with ID: {UserId} retrieved incomes grid. Total items after filtering: {TotalItemsCount}. " +
            "Current page: {CurrentPage}. Page size: {PageSize}.",
            userId,
            totalItems,
            gridRequest.Page,
            gridRequest.PageSize);

        return (new PagedResult<IncomeViewModel>
        {
            Items = items,
            Pager = pager,
            GridRequest = gridRequest,
            FilterModelType = typeof(Income),
            CustomFilters = customFilters
        },
        totalAmount);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        var income = await unitOfWork.IncomeRepository.GetByIdAsync(id, userId, ct);
        if (income is null)
        {
            logger.LogWarning("User with ID: {UserId} failed to delete income with ID: {IncomeId}. Income was not found or does not belong to the current user.",
                userId,
                id);

            throw new NotFoundException("Příjem nebyl nalezen.");
        }

        unitOfWork.IncomeRepository.Delete(income);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User with ID: {UserId} deleted income with ID: {IncomeId}.", userId, id);
    }

    public async Task AddAsync(IncomeUpsertViewModel model, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        await EnsureCanCreateIncomeAsync(userId, ct);

        var income = mapper.Map<Income>(model);

        unitOfWork.IncomeRepository.Add(income);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User with ID: {UserId} created income for household member with ID: {HouseholdMemberId}.",userId, model.HouseholdMemberId);
    }

    public async Task<IncomeUpsertViewModel> GetForCreateAsync(CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        await EnsureCanCreateIncomeAsync(userId, ct);

        var (householdMembersListItems, incomeCategoriesListItems) = await GetSelectOptionsAsync(userId, ct);

        return new IncomeUpsertViewModel
        {
            Date = DateTime.Now,
            HouseholdMembers = householdMembersListItems,
            IncomeCategories = incomeCategoriesListItems
        };
    }

    public async Task FillSelectOptionsAsync(IncomeUpsertViewModel model, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;
        var (householdMembersListItems, incomeCategoriesListItems) = await GetSelectOptionsAsync(userId, ct);

        model.HouseholdMembers = householdMembersListItems;
        model.IncomeCategories = incomeCategoriesListItems;
    }

    public async Task<IncomeUpsertViewModel> GetForUpdateAsync(int id, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        var income = await unitOfWork.IncomeRepository.GetByIdAsync(id, userId, ct);
        if (income is null)
        {
            logger.LogWarning("User with ID: {UserId} failed to update income with ID: {IncomeId}. Income was not found or does not belong to the current user.",
                userId,
                id);

            throw new NotFoundException("Příjem nebyl nalezen.");
        }

        var incomeUpsertVm = mapper.Map<IncomeUpsertViewModel>(income);

        var (householdMembersListItems, incomeCategoriesListItems) = await GetSelectOptionsAsync(userId, ct);

        var householdMemberIds = householdMembersListItems.Select(x => int.Parse(x.Value)).ToList();
        if (!householdMemberIds.Contains(income.HouseholdMemberId))
        {
            householdMembersListItems.Add(new SelectListItem
            {
                Text = income.HouseholdMember.Nickname, 
                Value = income.HouseholdMemberId.ToString() 
            });
        }

        var incomeCategoryIds = incomeCategoriesListItems.Select(x => int.Parse(x.Value)).ToList();         
        if (!incomeCategoryIds.Contains(income.IncomeCategoryId))
        {
            incomeCategoriesListItems.Add(new SelectListItem
            {
                Text = income.IncomeCategory.Name,
                Value = income.IncomeCategoryId.ToString()
            });
        }

        incomeUpsertVm.IncomeCategories = incomeCategoriesListItems;
        incomeUpsertVm.HouseholdMembers = householdMembersListItems;

        return incomeUpsertVm;
    }

    public async Task UpdateAsync(int id, IncomeUpsertViewModel model, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        if (id != model.Id)
        {
            logger.LogWarning("User with ID: {UserId} failed to update income because route ID: {RouteId} does not match model ID: {ModelId}.",
                userId,
                id,
                model.Id);

            throw new DomainException("Neplatný požadavek na úpravu příjmu.");
        }

        var income = await unitOfWork.IncomeRepository.GetByIdAsync(id, userId, ct);
        if (income is null)
        {
            logger.LogWarning("User with ID: {UserId} failed to update income with ID: {IncomeId}. Income was not found or does not belong to the current user.",
                userId,
                id);

            throw new NotFoundException("Příjem nebyl nalezen.");
        }

        var belongsHouseholdMemberToUser = await unitOfWork.HouseholdMemberRepository.BelongsToUserAndIsActiveAsync(model.HouseholdMemberId, userId, ct);
        if (!belongsHouseholdMemberToUser)
        {
            logger.LogWarning("User with ID: {UserId} failed to update income with ID: {IncomeId}. Household member with ID: {HouseholdMemberId} is not active or does not belong to the current user.",
                userId,
                id,
                model.HouseholdMemberId);

            throw new DomainException("Vybraný člen domácnosti není aktivní nebo nepatří aktuálnímu uživateli.");
        }

        var belongsIncomeCategoryToUser = await unitOfWork.IncomeCategoryRepository.BelongsToUserAndIsActiveAsync(model.IncomeCategoryId, userId, ct);
        if (!belongsIncomeCategoryToUser)
        {
            logger.LogWarning("User with ID: {UserId} failed to update income with ID: {IncomeId}. Income category with ID: {IncomeCategoryId} is not active or does not belong to the current user.",
                userId,
                id,
                model.IncomeCategoryId);

            throw new DomainException("Vybraná kategorie příjmu není aktivní nebo nepatří aktuálnímu uživateli.");
        }

        mapper.Map(model, income);

        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User with ID: {UserId} updated income with ID: {IncomeId} for household member with ID: {HouseholdMemberId}.",
            userId,
            id,
            model.HouseholdMemberId);
    }

    private async Task<(List<SelectListItem>, List<SelectListItem>)> GetSelectOptionsAsync(string userId, CancellationToken ct)
    {
        var houseHoldMembers = await unitOfWork.HouseholdMemberRepository.GetAllActiveAsync(userId, ct);
        var houseHoldMembersListItems = houseHoldMembers.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Nickname
        })
        .ToList();

        var incomeCategories = await unitOfWork.IncomeCategoryRepository.GetAllActiveAsync(userId, ct);
        var incomeCategoriesListItems = incomeCategories.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name
        })
        .ToList();

        return (houseHoldMembersListItems, incomeCategoriesListItems);
    }

    private async Task EnsureCanCreateIncomeAsync(string userId, CancellationToken ct)
    {
        var existsAnyActiveHouseholdMember = await unitOfWork.HouseholdMemberRepository.ExistsAnyActiveAsync(userId, ct);
        if (!existsAnyActiveHouseholdMember)
        {
            logger.LogWarning("User with ID: {UserId} failed to create income because no active household member exists.", userId);
            throw new DomainException("Není žádný aktivní člen domácnosti, nejprve jej aktivujte nebo vytvořte.");
        }

        var existsAnyActiveIncomeCategory = await unitOfWork.IncomeCategoryRepository.ExistsAnyActiveAsync(userId, ct);
        if (!existsAnyActiveIncomeCategory)
        {
            logger.LogWarning("User with ID: {UserId} failed to create income because no active income category exists.", userId);
            throw new DomainException("Není žádná aktivní kategorie příjmu, nejprve ji aktivujte nebo vytvořte.");
        }
    }
}
