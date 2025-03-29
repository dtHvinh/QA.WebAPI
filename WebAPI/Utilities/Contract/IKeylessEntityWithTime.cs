namespace WebAPI.Utilities.Contract;

public interface IKeylessEntityWithTime : IKeylessEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public interface IKeylessEntity;