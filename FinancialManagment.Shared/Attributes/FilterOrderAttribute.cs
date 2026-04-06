namespace FinancialManagment.Shared.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class FilterOrderAttribute : Attribute
{
    public int Order { get; }

    public FilterOrderAttribute(int order)
    {
        Order = order;
    }
}
