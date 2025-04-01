using WebAPI.Utilities.Contract;

namespace WebAPI.Model;

public class Report : IEntityWithTime<int>
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string Description { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Status { get; set; } = default!;
    public int TargetId { get; set; }
}
