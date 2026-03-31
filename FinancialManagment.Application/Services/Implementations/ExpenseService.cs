using AutoMapper;
using FinancialManagment.Application.Exceptions;
using FinancialManagment.Application.Models.Expense;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Application.UserIdentity;
using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Shared.Utilities;
using FinancialManagment.Shared.Pagination;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace FinancialManagment.Application.Services.Implementations;

public sealed class ExpenseService(
    ILogger<ExpenseService> logger,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IMapper mapper,
    IImageService imageService) : IExpenseService
{
    public async Task<ExpenseIndexViewModel> GetIndexAsync(
        PagedRequest request,
        int? householdMemberId,
        int? expenseCategoryId,
        DateTime? from,
        DateTime? to,
        CancellationToken ct)
    {
        request = request.Normalize();
        var userId = currentUser.ValidatedUserId;

        DateTime effectiveFrom = from ?? DateTime.Now.AddYears(-1);
        DateTime effectiveTo  = to ?? DateTime.Now;

        var (expenses, totalItemsCount, totalExpenseSum) = await unitOfWork.ExpenseRepository.GetAllAsync(
            request,
            householdMemberId,
            expenseCategoryId, 
            userId,
            effectiveFrom,
            effectiveTo,
            ct);

        var items = mapper.Map<List<ExpenseViewModel>>(expenses);

        var result = new PagedResult<ExpenseViewModel>
        { 
            Page = request.Page,
            PageSize = request.PageSize,
            Search = request.Search,
            SortBy = request.SortBy,
            Desc = request.Desc,
            Items = items,
            TotalItemsCount = totalItemsCount
        };

        var (houseHoldMembersListItems, expenseCategoriesListItems) = await GetSelectOptionsAsync(userId, ct);

        logger.LogInformation("User with ID: {UserId} retrieved expenses list. Total items: {TotalItemsCount}.", userId, totalItemsCount);

        return new ExpenseIndexViewModel
        {
            Result = result,
            ExpenseSum = totalExpenseSum,
            SortOptions = OptionsBuilder.GetExpenseOrIncomeOptions(true),
            HouseholdMemberOptions = houseHoldMembersListItems,
            ExpenseCategoryOptions = expenseCategoriesListItems
        };
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        var expense = await unitOfWork.ExpenseRepository.GetByIdAsync(id, userId, ct);
        if (expense is null)
        {
            logger.LogWarning("User with ID: {UserId} failed to delete expense with ID: {ExpenseId}. Expense was not found or does not belong to the current user.",
                userId,
                id);

            throw new NotFoundException("Výdaj nebyl nalezen.");
        }

        await imageService.DeleteAsync(expense.ReceiptFileName, ct);

        unitOfWork.ExpenseRepository.Delete(expense);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User with ID: {UserId} deleted expense with ID: {ExpenseId}.", userId, id);
    }

    public async Task<ExpenseUpsertViewModel> GetForCreateAsync(CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        await EnsureCanCreateExpenseAsync(userId, ct);

        var (householdMembersListItems, expenseCategoriesListItems) = await GetSelectOptionsAsync(userId, ct);

        return new ExpenseUpsertViewModel
        {
            Date = DateTime.Now,
            HouseholdMembers = householdMembersListItems,
            ExpenseCategories = expenseCategoriesListItems
        };
    }

    public async Task AddAsync(ExpenseUpsertViewModel model, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        await EnsureCanCreateExpenseAsync(userId, ct);

        var expense = mapper.Map<Expense>(model);

        expense.ReceiptFileName = await imageService.SaveAsync(model.ReceiptFile, ct);

        unitOfWork.ExpenseRepository.Add(expense);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User with ID: {UserId} created expense for household member with ID: {HouseholdMemberId}.", userId, model.HouseholdMemberId);
    }

    public async Task FillSelectOptionsAsync(ExpenseUpsertViewModel model, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;
        var (householdMembersListItems, expenseCategoriesListItems) = await GetSelectOptionsAsync(userId, ct);

        model.HouseholdMembers = householdMembersListItems;
        model.ExpenseCategories = expenseCategoriesListItems;
    }

    public async Task<ExpenseUpsertViewModel> GetForUpdateAsync(int id, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        var expense = await unitOfWork.ExpenseRepository.GetByIdAsync(id, userId, ct);
        if (expense is null)
        {
            logger.LogWarning("User with ID: {UserId} failed to update expense with ID: {ExpenseId}. Expense was not found or does not belong to the current user.",
                userId,
                id);

            throw new NotFoundException("Výdaj nebyl nalezen.");
        }

        var expenseUpsertVm = mapper.Map<ExpenseUpsertViewModel>(expense);

        var (householdMembersListItems, expenseCategoriesListItems) = await GetSelectOptionsAsync(userId, ct);

        var householdMemberIds = householdMembersListItems.Select(x => int.Parse(x.Value)).ToList();
        if (!householdMemberIds.Contains(expense.HouseholdMemberId))
        {
            householdMembersListItems.Add(new SelectListItem
            {
                Text = expense.HouseholdMember.Nickname,
                Value = expense.HouseholdMemberId.ToString()
            });
        }

        var expenseCategoryIds = expenseCategoriesListItems.Select(x => int.Parse(x.Value)).ToList();
        if (!expenseCategoryIds.Contains(expense.ExpenseCategoryId))
        {
            expenseCategoriesListItems.Add(new SelectListItem
            {
                Text = expense.ExpenseCategory.Name,
                Value = expense.ExpenseCategoryId.ToString()
            });
        }

        expenseUpsertVm.ExpenseCategories = expenseCategoriesListItems;
        expenseUpsertVm.HouseholdMembers = householdMembersListItems;

        return expenseUpsertVm;
    }

    public async Task UpdateAsync(int id, ExpenseUpsertViewModel model, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        if (id != model.Id)
        {
            logger.LogWarning("User with ID: {UserId} failed to update expense because route ID: {RouteId} does not match model ID: {ModelId}.",
                userId,
                id,
                model.Id);

            throw new DomainException("Neplatný požadavek na úpravu výdaje.");
        }

        var expense = await unitOfWork.ExpenseRepository.GetByIdAsync(id, userId, ct);
        if (expense is null)
        {
            logger.LogWarning("User with ID: {UserId} failed to update expense with ID: {ExpenseId}. Expense was not found or does not belong to the current user.",
                userId,
                id);

            throw new NotFoundException("Výdaj nebyl nalezen.");
        }

        var belongsHouseholdMemberToUser = await unitOfWork.HouseholdMemberRepository.BelongsToUserAndIsActiveAsync(model.HouseholdMemberId, userId, ct);
        if (!belongsHouseholdMemberToUser)
        {
            logger.LogWarning("User with ID: {UserId} failed to update expense with ID: {ExpenseId}. Household member with ID: {HouseholdMemberId} is not active or does not belong to the current user.",
                userId,
                id,
                model.HouseholdMemberId);

            throw new DomainException("Vybraný člen domácnosti není aktivní nebo nepatří aktuálnímu uživateli.");
        }

        var belongsExpenseCategoryToUser = await unitOfWork.ExpenseCategoryRepository.BelongsToUserAndIsActiveAsync(model.ExpenseCategoryId, userId, ct);
        if (!belongsExpenseCategoryToUser)
        {
            logger.LogWarning("User with ID: {UserId} failed to update expense with ID: {ExpenseId}. Expense category with ID: {ExpenseCategoryId} is not active or does not belong to the current user.",
                userId,
                id,
                model.ExpenseCategoryId);

            throw new DomainException("Vybraná kategorie výdaje není aktivní nebo nepatří aktuálnímu uživateli.");
        }

        mapper.Map(model, expense);

        if (model.ReceiptFile is not null)
        {
            await imageService.DeleteAsync(expense.ReceiptFileName, ct);
            expense.ReceiptFileName = await imageService.SaveAsync(model.ReceiptFile, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User with ID: {UserId} updated expense with ID: {ExpenseId} for household member with ID: {HouseholdMemberId}.",
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

        var expenseCategories = await unitOfWork.ExpenseCategoryRepository.GetAllActiveAsync(userId, ct);
        var expenseCategoriesListItems = expenseCategories.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name
        })
        .ToList();

        return (houseHoldMembersListItems, expenseCategoriesListItems);
    }

    private async Task EnsureCanCreateExpenseAsync(string userId, CancellationToken ct)
    {
        var existsAnyActiveHouseholdMember = await unitOfWork.HouseholdMemberRepository.ExistsAnyActiveAsync(userId, ct);
        if (!existsAnyActiveHouseholdMember)
        {
            logger.LogWarning("User with ID: {UserId} failed to create expense because no active household member exists.", userId);
            throw new DomainException("Není žádný aktivní člen domácnosti, nejprve jej aktivujte nebo vytvořte.");
        }

        var existsAnyActiveExpenseCategory = await unitOfWork.ExpenseCategoryRepository.ExistsAnyActiveAsync(userId, ct);
        if (!existsAnyActiveExpenseCategory)
        {
            logger.LogWarning("User with ID: {UserId} failed to create expense because no active expense category exists.", userId);
            throw new DomainException("Není žádná aktivsní kategorie výdaje, nejprve ji aktivujte nebo vytvořte.");
        }
    }

    public async Task<(bool, string)> DeleteImageAsync(int id, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        var expense = await unitOfWork.ExpenseRepository.GetByIdAsync(id, userId, ct);
        if (expense is null)
        {
            logger.LogWarning("User with ID: {UserId} failed to delete image of expense with ID: {ExpenseId}. " +
                "Expense was not found or does not belong to the current user.",
                userId,
                id);

            return (false, "Výdaj nebyl nalezen, nejde upravit obrázek.");
        }

        if (string.IsNullOrWhiteSpace(expense.ReceiptFileName))
        {
            return (false, "Obrázek nelze  odstranit, nebyl nalezen.");
        }

        try
        {
            await imageService.DeleteAsync(expense.ReceiptFileName, ct);
            expense.ReceiptFileName = null;
            await unitOfWork.SaveChangesAsync(ct);

            return (true, "Obrázek úspěšně odstraněn.");
        }
        catch (Exception ex)
        {
            logger.LogError("User with ID: {UserId} failed to delete image of expense with ID: {ExpenseId}. Error: {Error}.",
                userId,
                id,
                ex.Message);

            return (false, "Nepodařilo se odstranit obrázek, kontaktujte administrátora webu.");
        }
    }
}
