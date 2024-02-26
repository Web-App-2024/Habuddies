using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HaBuddies.Models;

public class Post
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }

    public required string title { get; set; }

    public string? description { get; set; } = null!;

    public required string category { get; set; }

    public required int enrollSize { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public required DateTime endDate { get; set; }

    public string? coverImage { get; set; } = null!;

    public bool? isOpen { get; set; } = true;
}