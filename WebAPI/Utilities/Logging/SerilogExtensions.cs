using Serilog.Events;
using WebAPI.Model;
using WebAPI.Utilities.Contract;

namespace WebAPI.Utilities.Logging;

/*
 * User {UserId} {Operator} {EntityType} {Id}
 * User {UserId} {Operator} {EntityType} {Id} {ModeratorOperator} by moderator {ModeratorId}
 */

public static class SerilogExtensions
{
    public const string UserActionMessageTemplate = "User #{UserId} {Operator} {EntityType} #{@EntityId}";
    public const string ModeratorActionMessageTemplate = "User #{UserId} {Operator} {EntityType} #{@EntityId} {ModeratorOperator} by moderator #{ModeratorId}";
    public const string ModeratorActionNoEntityOwnerMessageSimple = "{EntityType} #{@EntityId} {ModeratorOperator} by moderator #{ModeratorId}";

    public const string PropertyUserId = "UserId";
    public const string PropertyOperator = "Operator";
    public const string PropertyEntityType = "EntityType";
    public const string PropertyEntityId = "EntityId";
    public const string PropertyModeratorOperator = "ModeratorOperator";
    public const string PropertyModeratorId = "ModeratorId";

    public static void UserAction(this Serilog.ILogger logger, LogEventLevel level,
        int userId, LogOp op, IEntity<int> target)
    {
        logger.Write(level, UserActionMessageTemplate,
            userId, op, target.GetType().Name, target.Id != default ? target.Id : target);
    }

    public static void UserAction(this Serilog.ILogger logger, LogEventLevel level,
        int userId, LogOp op, Type targetType, int targetId)
    {
        logger.Write(level, UserActionMessageTemplate,
            userId, op, targetType.GetType().Name, targetId);
    }

    /// <summary>
    /// "User {UserId} {Operator} {EntityType} {EntityId} {ModeratorOperator} by moderator {ModeratorId}";
    /// </summary>
    public static void ModeratorAction(this Serilog.ILogger logger, LogEventLevel level,
    int moderatorId, LogModeratorOp moderatorOp, AppUser user, LogOp op, IEntity<int> target)
    {
        logger.Write(level, ModeratorActionMessageTemplate,
            user.Id, op, target.GetType().Name, target.Id != default ? target.Id : target, moderatorOp, moderatorId);
    }

    /// <summary>
    ///  "{EntityType} {EntityId} {ModeratorOperator} by moderator {ModeratorId}";
    /// </summary>
    public static void ModeratorNoEnityOwnerAction(this Serilog.ILogger logger, LogEventLevel level,
    int moderatorId, LogModeratorOp moderatorOp, IEntity<int> target)
    {
        logger.Write(level, ModeratorActionNoEntityOwnerMessageSimple,
            target.GetType().Name, target.Id != default ? target.Id : target, moderatorOp, moderatorId);
    }
}

public enum LogOpRes
{
    Successfully,
    Failed
}

public enum LogOp
{
    RevokedCommunityMod,
    GrantCommunityMod,
    Liked,
    Left,
    Removed,
    Joined,
    Created,
    Updated,
    Deleted,
    Search
}

public enum LogModeratorOp
{
    Approved,
    Rejected,
    Delete,
    Reopen
}