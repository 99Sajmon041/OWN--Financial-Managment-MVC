using AutoMapper;
using FinancialManagment.Application.Models.Expense;
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

        CreateMap<Income, IncomeUpsertViewModel>();

        CreateMap<IncomeUpsertViewModel, Income>()
            .ForMember(x => x.Id, opt => opt.Ignore());

        CreateMap<Expense, ExpenseViewModel>()
            .ForMember(x => x.HouseholdMemberNickname, opt => opt.MapFrom(x => x.HouseholdMember.Nickname))
            .ForMember(x => x.ExpenseCategoryName, opt => opt.MapFrom(x => x.ExpenseCategory.Name));

        CreateMap<Expense, ExpenseUpsertViewModel>();

        CreateMap<ExpenseUpsertViewModel, Expense>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.ReceiptFileName, opt => opt.Ignore());
    }
}
