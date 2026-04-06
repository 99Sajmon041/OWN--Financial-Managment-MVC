using FinancialManagment.Shared.Grid.Filtering;

namespace FinancialManagment.Shared.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class FilterTypeAttribute : Attribute
{
    public FilterInputType InputType { get; }

    public FilterTypeAttribute(FilterInputType inputType)
    {
        InputType = inputType;
    }
}