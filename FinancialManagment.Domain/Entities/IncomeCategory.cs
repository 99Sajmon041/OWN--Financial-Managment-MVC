using FinancialManagment.Domain.EntityInterface;

namespace FinancialManagment.Domain.Entities;

public sealed class IncomeCategory : BaseCategory
{
    public ICollection<Income> Incomes { get; set; } = [];
}
