using AutoMapper;
using FinancialManagment.Application.Models.HouseholdMember;
using FinancialManagment.Application.Models.IncomeCategory;
using FinancialManagment.Domain.Entities;

namespace FinancialManagment.Application.Mapping;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<IncomeCategory, IncomeCategoryViewModel>();
        CreateMap<IncomeCategory, IncomeCategoryUpsertViewModel>();

        CreateMap<HouseholdMember, HouseholdMemberViewModel>();
        CreateMap<HouseholdMemberUpsertViewModel, HouseholdMember>();
    }
}
