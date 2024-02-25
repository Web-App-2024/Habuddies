using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace HaBuddies.Models {
    public class User {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? id { get; set; }
        public string name { get; set; } = null!;
        public string surname { get; set; } = null!;
        public string email { get; set; } = null!;
        public string password { get; set; } = null!;
    }

    public class UpdateUser {
        public string name { get; set; } = null!;
        public string surname { get; set; } = null!;
        public string password { get; set; } = null!;
    }
    
    public class UserNoPassword {
        public static explicit operator UserNoPassword(User obj) {
            return JsonConvert.DeserializeObject<UserNoPassword>(JsonConvert.SerializeObject(obj));
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? id { get; set; }
        public string name { get; set; } = null!;
        public string surname { get; set; } = null!;
        public string email { get; set; } = null!;
    }

    public class UserDto {
        public string email { get; set; } = null!;
        public string password { get; set; } = null!;
    }
}