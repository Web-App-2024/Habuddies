public class PaginationParams
{
    public int PerPage { get; set; }
    public int Page { get; set; }
    public int Skip { get; set; }
}

public class PaginationResponse<T>
{
    public List<T> Data { get; set; } = [];
    public int CurrentPage { get; set; }
    public int? NextPage { get; set; }
    public int? PrevPage { get; set; }
    public int Count { get; set; }
    public int PageCount { get; set; }
}

public static class Pagination
{
    public static PaginationParams BuildPaginationLimit(int page, int perPage, int limit)
    {
        perPage = Math.Min(perPage, limit);

        int skip = (page - 1) * perPage;

        return new PaginationParams
        {
            Page = page,
            PerPage = perPage,
            Skip = skip
        };
    }

    public static PaginationResponse<T> BuildResponsePagination<T>(List<T> data, int page, int perPage, int totalCount)
    {
        int currentPage = page;
        int totalPages = (int)Math.Ceiling((double)totalCount / perPage);

        bool hasNextPage = currentPage < totalPages;
        bool hasPrevPage = currentPage > 1;

        int? nextPage = hasNextPage ? currentPage + 1 : null;
        int? prevPage = hasPrevPage ? currentPage - 1 : null;

        return new PaginationResponse<T>
        {
            Data = data,
            CurrentPage = currentPage,
            NextPage = nextPage,
            PrevPage = prevPage,
            Count = totalCount,
            PageCount = totalPages
        };
    }
}