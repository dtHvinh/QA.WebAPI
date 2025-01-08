namespace WebAPI.Utilities.Contract;

public interface IKeylessEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
