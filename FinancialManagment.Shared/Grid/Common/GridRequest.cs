namespace FinancialManagment.Shared.Grid.Common;

public sealed class GridRequest
{
    public Dictionary<string, string> Filters = [];
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortOrder { get; set; }
    public void Normalize()
    {
        if (Page < 1)
        {
            Page = 1;
        }

        if (PageSize < 1)
        {
            PageSize = 10;
        }
    }
}
