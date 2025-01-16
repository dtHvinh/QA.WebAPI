namespace WebAPI.Pagination;

public class PagedResponse<T>(IEnumerable<T> items, bool hasNext, int pageNumber, int pageSize)
{
    public int PageIndex { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
    public bool HasNextPage { get; set; } = hasNext;
    public bool HasPreviousPage { get; set; } = pageNumber > 1;
    public IEnumerable<T> Items { get; set; } = items;

    public int Skip => (PageIndex - 1) * PageSize;
    public int Take => PageSize;
}
