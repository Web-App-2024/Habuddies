using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HaBuddies.Models;

public class NotificationDTO
{
    public required string Content { get; set; }
    public required string UserId { get; set; }
    public required string EventId { get; set; }
}