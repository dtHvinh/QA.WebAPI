namespace WebAPI.Utilities.Contract;

public interface IEntityWithTime<T> : IEntity<T>
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public interface IEntity<T> where T : allows ref struct
{
    public T Id { get; set; }
}
