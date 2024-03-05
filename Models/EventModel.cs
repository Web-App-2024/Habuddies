using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HaBuddies.Models;

public class Event
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string OwnerId { get; set; }
    public required string Category { get; set; }
    public int EnrollSize { get; set; }
    public string Description { get; set; } = "";
    public DateTime EndDate { get; set; }
    public bool IsOpen { get; set; } = true;
    public List<string> SubscribersId { get; set; } = [];
    public List<string> QueueId { get; set; } = [];
    public int? MinAgeRequirement { get; set; }
    public int? MaxAgeRequirement { get; set; }
    public List<string> GenderRequirement { get; set; } = [];

    [BsonIgnore]
    public required User Owner { get; set; }
    [BsonIgnore]
    public List<User> Subscribers { get; set; } = [];
    [BsonIgnore]
    public List<User> Queue { get; set; } = [];
}