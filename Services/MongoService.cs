using HaBuddies.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HaBuddies.Services;
public class MongoService {
    public readonly IMongoCollection<Post> _postsCollection;

    public MongoService(IOptions<HaBuddiesDatabaseSettings> mongoDBSettings) 
    {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionString);
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _postsCollection = database.GetCollection<Post>("Posts");
    }
}
