namespace FinancialManagment.Shared.Pagination;

public static class PagingValidator
{
    public static PagedRequest Normalize(this PagedRequest request)
    {
        if (request.Page <= 0)
            request.Page = 1;

        if (request.PageSize is not (5 or 10 or 20 or 50))
            request.PageSize = 10;
        
        request.Search = request.Search?.Trim();

        if (!string.IsNullOrWhiteSpace(request.Search) && request.Search.Length > 50)
            request.Search = request.Search.Substring(0, 50);

        return request;
    }
}
