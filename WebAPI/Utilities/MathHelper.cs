namespace WebAPI.Utilities;

public static class MathHelper
{
    public static int GetTotalPage(int totalCount, int pageSize)
    {
        return (int)Math.Ceiling((double)totalCount / pageSize);
    }

    public static double GetPercentageGrowth(int previousCount, int currentCount)
    {
        if (previousCount == 0)
        {
            return currentCount == 0 ? 0 : 100;
        }
        return Math.Round(((double)currentCount - previousCount) / previousCount * 100, 2);
    }
}
