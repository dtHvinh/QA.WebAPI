using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAPI.Model;

public class SysLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    [BsonElement("Timestamp")]
    public DateTime Timestamp { get; set; }

    public string Level { get; set; } = default!;

    public string MessageTemplate { get; set; } = default!;

    public string RenderedMessage { get; set; } = default!;

    public object Properties { get; set; } = default!;

    public string UtcTimestamp { get; set; } = default!;
}

public class LogProperties
{
    public int UserId { get; set; }
    public string Operator { get; set; } = default!;
    public string EntityType { get; set; } = default!;
    public int EntityId { get; set; }
}

