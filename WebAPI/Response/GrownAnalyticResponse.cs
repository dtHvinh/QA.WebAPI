namespace WebAPI.Response;

public record GrownAnalyticResponse(int LastWeekCount, int CurrentWeekCount, double GrowthPercentage);
