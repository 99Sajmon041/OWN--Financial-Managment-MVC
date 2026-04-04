namespace FinancialManagment.Shared.Grid;

public sealed class Pager
{
    public int TotalItems { get; private set; }
    public int CurrentPage { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPages { get; private set; }
    public int StartPage { get; private set; }
    public int EndPage { get; private set; }

    public Pager(int totalItems, int currentPage, int pageSize)
    {
        if (pageSize < 1)
        {
            pageSize = 10;
        }

        if (currentPage < 1)
        {
            currentPage = 1;
        }

        TotalItems = totalItems;
        CurrentPage = currentPage;
        PageSize = pageSize;

        TotalPages = (int)Math.Ceiling(totalItems / (decimal)pageSize);

        int startPage = currentPage - 5;
        int endPage = currentPage + 4;

        if (startPage <= 0)
        {
            endPage = endPage - (startPage - 1);
            startPage = 1;
        }

        if (endPage > TotalPages)
        {
            endPage = TotalPages;

            if (endPage > 10)
            {
                startPage = endPage - 9;
            }
        }

        if (TotalPages == 0)
        {
            startPage = 0;
            endPage = 0;
        }

        StartPage = startPage;
        EndPage = endPage;
    }

    public int GetSkip()
    {
        return (CurrentPage - 1) * PageSize;
    }
}