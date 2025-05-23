namespace WebAPI.Utilities.Contract;

public interface IKeylessEntityWithTime : IKeylessEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public interface IKeylessEntity;