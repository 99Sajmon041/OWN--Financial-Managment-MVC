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

        var (incomes, totalItemsCount) = await unitOfWork.IncomeRepository.GetAllAsync(request, householdMemberId, incomeCategoryId, _from, _to, ct);

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
            logger.LogWarning("User with ID: {UserId} failed to delete income with ID: {IncomeId}. Income was not found or does not belong to household member.",
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

        var existsAnyActiveHouseHoldMember = await unitOfWork.HouseholdMemberRepository.ExistsAnyActiveAsync(userId, ct);
        if (!existsAnyActiveHouseHoldMember)
        {
            logger.LogWarning("User with ID: {UserId} failed to create income because no active household member exists.", userId);

            throw new DomainException("Není žádný aktivní člen domácnosti, nejprve jej aktivujte nebo vytvořte.");
        }

        var income = mapper.Map<Income>(model);

        unitOfWork.IncomeRepository.Add(income);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User with ID: {UserId} created income for household member with ID: {HouseholdMemberId}.",userId, model.HouseholdMemberId);
    }
}
