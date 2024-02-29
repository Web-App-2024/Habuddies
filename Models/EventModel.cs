using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HaBuddies.Models;

public class Event
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Title { get; set; }
    public string OwnerId { get; set; }
    public string Category { get; set; }
    public int EnrollSize { get; set; }
    public string Description { get; set; }
    public DateTime EndDate { get; set; }
    public string CoverImage { get; set; }
    public bool IsOpen { get; set; }
    public List<string> Subscribers { get; set; }
    public List<string> Queue { get; set; }
    public int AgeRequirement { get; set; }
    public string GenderRequirement { get; set; }
    public DateTime StartEvent { get; set; }
    public DateTime EndEvent { get; set; }
}