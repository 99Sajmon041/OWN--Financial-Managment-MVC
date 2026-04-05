namespace FinancialManagment.Shared.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class FilterLabelAttribute : Attribute
{
    public string Label { get; }

    public FilterLabelAttribute(string label)
    {
        Label = label;
    }
}