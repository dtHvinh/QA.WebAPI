namespace WebAPI.Pagination;

public class PageArgs
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }

    public static PageArgs From(int pageIndex, int pageSize)
    {
        return new PageArgs
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public int CalculateSkip()
    {
        return (PageIndex - 1) * PageSize;
    }
}
