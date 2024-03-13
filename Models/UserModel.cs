using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace HaBuddies.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public string[]? JoinedEvent { get; set;} = null;
    }

    public class UserNoPassword
    {
        public static explicit operator UserNoPassword(User obj)
        {
            return JsonConvert.DeserializeObject<UserNoPassword>(JsonConvert.SerializeObject(obj))!;
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = null!;
        public string Bio { get; set; } = null!;
    }
}