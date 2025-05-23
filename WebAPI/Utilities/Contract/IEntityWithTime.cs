namespace WebAPI.Utilities.Contract;

public interface IEntityWithTime<T> : IEntity<T>
{
    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset ModificationDate { get; set; }
}

public interface IEntity<T> where T : allows ref struct
{
    public T Id { get; set; }
}
