using FinancialManagment.Shared.Attributes;

namespace FinancialManagment.Domain.Entities;

[FilterGroup("Výdaj")]
public sealed class Expense
{
    [FilterOrder(1)]
    [FilterLabel("ID výdaje")]
    public int Id { get; set; }
    public HouseholdMember HouseholdMember { get; set; } = default!;

    [NotFilterable]
    public int HouseholdMemberId { get; set; }
    public ExpenseCategory ExpenseCategory { get; set; } = default!;

    [NotFilterable]
    public int ExpenseCategoryId { get; set; }

    [FilterOrder(3)]
    [FilterLabel("Částka výdaje")]
    public decimal Amount { get; set; }

    [FilterOrder(2)]
    [FilterLabel("Datum výdaje")]
    public DateTime Date { get; set; }

    [FilterOrder(4)]
    [FilterLabel("Popis výdaje")]
    public string? Description { get; set; }

    [NotFilterable]
    public string? ReceiptFileName { get; set; }
}
