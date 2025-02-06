namespace WebAPI.Utilities;

public static class NumericCalcHelper
{
    public static int GetTotalPage(int totalCount, int pageSize)
    {
        return (int)Math.Ceiling((double)totalCount / pageSize);
    }
}
