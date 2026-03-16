using FinancialManagment.Domain.Entities;

namespace FinancialManagment.Domain.EntityInterface;

public abstract class BaseCategory
{
    public int Id { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = default!;
    public string ApplicationUserId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; } = true;
}
