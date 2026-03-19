using AutoMapper;
using FinancialManagment.Application.Models.ExpenseCategory;
using FinancialManagment.Application.Models.HouseholdMember;
using FinancialManagment.Application.Models.Income;
using FinancialManagment.Application.Models.IncomeCategory;
using FinancialManagment.Domain.Entities;

namespace FinancialManagment.Application.Mapping;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<IncomeCategory, IncomeCategoryViewModel>();
        CreateMap<IncomeCategory, IncomeCategoryUpsertViewModel>();

        CreateMap<ExpenseCategory, ExpenseCategoryViewModel>();
        CreateMap<ExpenseCategory, ExpenseCategoryUpsertViewModel>();

        CreateMap<HouseholdMember, HouseholdMemberViewModel>();
        CreateMap<HouseholdMemberUpsertViewModel, HouseholdMember>().ReverseMap();

        CreateMap<Income, IncomeViewModel>()
            .ForMember(x => x.HouseholdMemberNickname, opt => opt.MapFrom(x => x.HouseholdMember.Nickname))
            .ForMember(x => x.IncomeCategoryName, opt => opt.MapFrom(x => x.IncomeCategory.Name));

        CreateMap<IncomeUpsertViewModel, Income>()
            .ForMember(x => x.Id, opt => opt.Ignore());
    }
}
