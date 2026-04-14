using FinancialManagment.Shared.Attributes;

namespace FinancialManagment.Domain.Entities;

[FilterGroup("Příjem")]
public sealed class Income
{
    [NotFilterable]
    public int Id { get; set; }
    public HouseholdMember HouseholdMember { get; set; } = default!;

    [NotFilterable]
    public int HouseholdMemberId { get; set; }
    public IncomeCategory IncomeCategory { get; set; } = default!;

    [NotFilterable]
    public int IncomeCategoryId { get; set; }

    [FilterOrder(3)]
    [FilterLabel("Částka příjmu")]
    public decimal Amount { get; set; }

    [FilterOrder(4)]
    [FilterLabel("Datum příjmu")]
    public DateTime Date { get; set; }

    [FilterOrder(5)]
    [FilterLabel("Popis příjmu")]
    public string? Description { get; set; }
}
