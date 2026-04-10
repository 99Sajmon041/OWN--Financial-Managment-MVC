namespace FinancialManagment.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class FilterGroupAttribute : Attribute
{
    public string GroupName { get; }

    public FilterGroupAttribute(string groupName)
    {
        GroupName = groupName;
    }
}
