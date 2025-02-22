namespace WebAPI.Pagination;

public class PageArgs
{
    public int Page { get; set; }
    public int PageSize { get; set; }

    public static PageArgs From(int page, int pageSize)
    {
        return new PageArgs
        {
            Page = page,
            PageSize = pageSize
        };
    }

    public int CalculateSkip()
    {
        return (Page - 1) * PageSize;
    }
}
