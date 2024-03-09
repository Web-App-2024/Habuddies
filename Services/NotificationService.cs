using HaBuddies.Models;
using MongoDB.Driver;

namespace HaBuddies.Services
{
    public class NotificationService
    {
        private readonly MongoService _mongoService;
        private readonly IMongoCollection<Notification> _notificationsCollection;
        private readonly IMongoCollection<Event> _eventsCollection;
        private readonly IMongoCollection<User> _usersCollection;

        public NotificationService(MongoService mongoService)
        {
            _mongoService = mongoService;
            _notificationsCollection = _mongoService._notificationsCollection;
            _eventsCollection = _mongoService._eventsCollection;
            _usersCollection = _mongoService._userCollection;
        }

        public async Task<List<Notification>> GetAllAsync(string userId)
        {
            try 
            {
                FilterDefinition<Notification> filter = FilterDefinition<Notification>.Empty;

                if (!string.IsNullOrEmpty(userId))
                    filter = Builders<Notification>.Filter.Eq(noti => noti.UserId, userId);

                var sortDefinition = Builders<Notification>.Sort.Descending(noti => noti.CreatedAt);

                var notifications = await _notificationsCollection.Find(filter)
                                                .Sort(sortDefinition)
                                                .ToListAsync();

                foreach(var noti in notifications)
                {
                    var user = await _usersCollection.Find(u => u.Id == noti.UserId).FirstOrDefaultAsync();
                    UserNoPassword userNoPassword = (UserNoPassword)user;
                    noti.User = userNoPassword;
                    var evt = await _eventsCollection.Find(evt => evt.Id == noti.EventId).FirstOrDefaultAsync();
                    noti.Event = evt;
                }

                return notifications;
            }
            catch (Exception) 
            {
                throw;
            }
        }
    }
}
