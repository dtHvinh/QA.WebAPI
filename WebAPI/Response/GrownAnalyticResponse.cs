namespace WebAPI.Response;

public record GrownAnalyticResponse(int PreviousCount, int CurrentCount, double GrowthPercentage);
