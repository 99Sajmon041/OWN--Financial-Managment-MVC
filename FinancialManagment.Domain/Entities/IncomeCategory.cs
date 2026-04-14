using FinancialManagment.Domain.EntityInterface;
using FinancialManagment.Shared.Attributes;

namespace FinancialManagment.Domain.Entities;

[FilterGroup("Kategorie příjmu")]
public sealed class IncomeCategory : BaseCategory
{
    public ICollection<Income> Incomes { get; set; } = [];
}
