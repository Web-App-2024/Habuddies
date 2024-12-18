using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HaBuddies.Models;

public class Notification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }
    public required TypeStatus Type { get; set; }
    public required string UserId { get; set; }
    public required string FromUserId { get; set; }
    public required string EventId { get; set; }
    public required bool IsHost { get; set; }
    public required bool IsViewed { get; set; }

    [BsonIgnore]
    public required UserNoPassword User { get; set; }
    [BsonIgnore]
    public required Event Event { get; set; }
    
    [BsonRepresentation(BsonType.DateTime)]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum TypeStatus
{
    Deleted,
    Joined,
    Cancelled,
    ClosedJoin,
    ClosedQueue
}