using FinancialManagment.Domain.Entities;
using FinancialManagment.Shared.Attributes;

namespace FinancialManagment.Domain.EntityInterface;

public abstract class BaseCategory
{
    [NotFilterable]
    public int Id { get; set; }

    [NotFilterable]
    public ApplicationUser ApplicationUser { get; set; } = default!;

    [NotFilterable]
    public string ApplicationUserId { get; set; } = default!;

    [FilterOrder(1)]
    [FilterLabel("Název kategorie")]
    public string Name { get; set; } = default!;

    [FilterOrder(2)]
    [FilterLabel("Aktivní kategorie")]
    public bool IsActive { get; set; } = true;
}
