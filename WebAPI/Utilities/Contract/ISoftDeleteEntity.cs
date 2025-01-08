namespace WebAPI.Utilities.Contract;

/// <summary>
/// Entity will be excluded by query result but still in database.
/// </summary>
public interface ISoftDeleteEntity
{
    bool IsDeleted { get; set; }
}
