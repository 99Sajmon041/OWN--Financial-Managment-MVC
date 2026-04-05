using AutoMapper;
using FinancialManagment.Application.Exceptions;
using FinancialManagment.Application.Models.HouseholdMember;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Application.UserIdentity;
using FinancialManagment.Domain.Entities;
using FinancialManagment.Domain.RepositoryInterfaces;
using FinancialManagment.Shared.Grid;
using FinancialManagment.Shared.Grid.Common;
using FinancialManagment.Shared.Grid.Paging;
using FinancialManagment.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinancialManagment.Application.Services.Implementations;

public sealed class HouseholdMemberService(
    ILogger<HouseholdMemberService> logger,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IMapper mapper) : IHouseholdMemberService
{
    public async Task<List<HouseholdMemberViewModel>> GetAllAsync(CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        var householdMembers = await unitOfWork.HouseholdMemberRepository.GetAllAsync(userId, ct);

        var householdMembersVm = mapper.Map<List<HouseholdMemberViewModel>>(householdMembers);

        logger.LogInformation("User with ID: {UserId} retrieved all household members to list. Total items: {TotalItemsCount}.", userId, householdMembersVm.Count);

        return householdMembersVm;
    }

    public async Task AddAsync(HouseholdMemberUpsertViewModel model, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        var membersCount = await unitOfWork.HouseholdMemberRepository.GetCountOfMembersAsync(userId, ct);
        if (membersCount >= 5)
        {
            logger.LogWarning("User with ID: {UserId} attempted to add a new household member, but maximum of house members is equal to 5.", userId);
            throw new DomainException("Nelze přidat dalšího člena, maximální počet členů je 5.");
        }

        model.Nickname = model.Nickname.Trim();

        var existing = await unitOfWork.HouseholdMemberRepository.ExistsByNameAsync(userId, model.Nickname, ct);
        if (existing)
        {
            logger.LogWarning("User with ID: {UserId} tries to add a new household member with name: {HouseHoldMemberNickname}, but that name already exists.",
                userId,
                model.Nickname);

            throw new ConflictException("Člen domácnosti s touto přezdívkou již existuje, zvolte jinou.");
        }

        var houseHoldMemberEntity = mapper.Map<HouseholdMember>(model);

        houseHoldMemberEntity.ApplicationUserId = userId;
        houseHoldMemberEntity.IsActive = true;

        unitOfWork.HouseholdMemberRepository.Add(houseHoldMemberEntity);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User with ID: {UserId} added a new household member with name: {HouseHoldMemberNickname}.", userId, model.Nickname);
    }

    public async Task UpdateAsync(int id, HouseholdMemberUpsertViewModel model, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;
        model.Nickname = model.Nickname.Trim();

        if (id != model.Id)
        {
            logger.LogWarning("User with ID: {UserId} attempted to update household member, but route ID does not match model ID.", userId);
            throw new DomainException("ID parametr se neshoduje.");
        }

        var householdMember = await unitOfWork.HouseholdMemberRepository.GetByIdAsync(id, userId, ct);
        if (householdMember is null)
        {
            logger.LogWarning("User with ID: {UserId} attempted to update household member with ID: {HouseHoldMemberId}, but it was not found.", userId, id);
            throw new DomainException($"Člen domácnosti s ID: {id} nebyl nalezen.");
        }

        var existsByName = await unitOfWork.HouseholdMemberRepository.ExistsByNameWithDifferentIdAsync(model.Nickname, id, userId, ct);

        if (existsByName)
        {
            logger.LogWarning("User with ID: {UserId} attempted to update household member with nickname: {HouseHoldMemberNickname}, but that name already exists.",
                userId, 
                model.Nickname);

            throw new ConflictException($"Člen domácnosti s přezdívkou: {model.Nickname} již existuje.");
        }

        householdMember.Nickname = model.Nickname;
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User with ID: {UserId} updated household member with ID: {HouseHoldMemberId}.", userId, id);
    }

    public async Task<HouseholdMemberUpsertViewModel?> GetByIdAsync(int id, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        var householdMember = await unitOfWork.HouseholdMemberRepository.GetByIdAsync(id, userId, ct);
        if (householdMember is null)
        {
            logger.LogWarning("User with ID: {UserId} attempted to get household member with ID: {HouseHoldMemberId}, but it was not found.", userId, id);
            throw new DomainException($"Člen domácnosti s ID: {id} nebyl nalezen.");
        }

        return mapper.Map<HouseholdMemberUpsertViewModel>(householdMember);
    }

    public async Task ChangeStatusAsync(int id, CancellationToken ct)
    {
        var userId = currentUser.ValidatedUserId;

        var householdMember = await unitOfWork.HouseholdMemberRepository.GetByIdAsync(id, userId, ct);
        if (householdMember is null)
        {
            logger.LogWarning("User with ID: {UserId} attempted to change status of household member with ID: {HouseHoldMemberId}, but it was not found.", userId, id);
            throw new DomainException($"Člen domácnosti s ID: {id} nebyl nalezen.");
        }

        string action;
        if (householdMember.IsActive)
        {
            householdMember.IsActive = false;
            action = "deactivated";
        }
        else
        {
            householdMember.IsActive = true;
            action = "activated";
        }


        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User with ID: {UserId} {Phrase} household member with ID: {HouseHoldMemberId}.", userId, action, id);
    }


    //Service method for grid with dynamic filtering, sorting, and pagination
    public async Task<PagedResultNew<HouseholdMemberViewModel>> GetGridAsync(GridRequest gridRequest, CancellationToken ct)
    {
        gridRequest.Normalize();

        var userId = currentUser.ValidatedUserId;

        IQueryable<HouseholdMember> query = unitOfWork.HouseholdMemberRepository.GetQueryable(userId);

        query = query.ApplyFilters(gridRequest.Filters);

        int totalItems = await query.CountAsync(ct);

        var pager = new Pager(totalItems, gridRequest.Page, gridRequest.PageSize);

        query = query.ApplySorting(gridRequest.SortOrder);
        query = query.ApplyPaging(pager);

        List<HouseholdMember> householdMembers = await query.ToListAsync(ct);

        IReadOnlyList<HouseholdMemberViewModel> items = mapper.Map<List<HouseholdMemberViewModel>>(householdMembers);

        logger.LogInformation("User with ID: {UserId} retrieved household members grid. Total items after filtering: {TotalItemsCount}. " +
            "Current page: {CurrentPage}. Page size: {PageSize}.",
            userId,
            totalItems,
            gridRequest.Page,
            gridRequest.PageSize);

        return new PagedResultNew<HouseholdMemberViewModel>
        {
            Items = items,
            Pager = pager,
            GridRequest = gridRequest,
            FilterModelType = typeof(HouseholdMember)
        };
    }
}
