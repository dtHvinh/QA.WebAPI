namespace WebAPI.Utilities.Contract;

public interface IKeylessEntityWithTime
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
