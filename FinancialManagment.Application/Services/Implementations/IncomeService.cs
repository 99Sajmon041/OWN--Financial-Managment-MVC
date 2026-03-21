using AutoMapper;
using FinancialManagment.Application.Exceptions;
using FinancialManagment.Application.Models.Income;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Application.UserIdentity;
using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Shared.Pagination;
using FinancialManagment.Web.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace FinancialManagment.Application.Services.Implementations;

public sealed class IncomeService(
    ILogger<IncomeService> logger,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IMapper mapper) : IIncomeService
{
    public async Task<IncomeIndexViewModel> GetIndexAsync(
        PagedRequest request,
        int? householdMemberId,
        int? incomeCategoryId,
        DateTime? from, 
        DateTime? to, 
        CancellationToken ct)
    {
        request = request.Normalize();
        var userId = currentUser.ValidatedUserId;

        DateTime _from = from ?? DateTime.Now.AddYears(-1);
        DateTime _to = to ?? DateTime.Now;

        var (incomes, totalItemsCount) = await unitOfWork.IncomeRepository.GetAllAsync(request, householdMemberId, incomeCategoryId, userId, _from, _to, ct);

        var items = mapper.Map<List<IncomeViewModel>>(incomes);

        var result = new PagedResult<IncomeViewModel>
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Search = request.Search,
            SortBy = request.SortBy,
            Desc = request.Desc,
            Items = items,
            TotalItemsCount = totalItemsCount
        };

        var (houseHoldMembersListItems, incomeCategoriesListItems) = await GetSelectOptionsAsync(userId, ct);

        logger.LogInformation("User with ID: {UserId} retrieved incomes list. Total items: {TotalItemsCount}.", userId, totalItemsCount);

        return new IncomeIndexViewModel
        {
            Result = result,
            SortOptions = OptionsBuilder.GetExpenseOrIncomeOptions(false),
            HouseholdMemberOptions = houseHoldMembersListItems,
            IncomeCategoryOptions = incomeCategoriesListItems
        };
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
