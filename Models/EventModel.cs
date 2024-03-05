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
    public List<string> Subscribers { get; set; } = [];
    public List<string> Queue { get; set; } = [];
    public int? AgeRequirement { get; set; }
    public string? GenderRequirement { get; set; }
}