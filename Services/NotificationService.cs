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

        public async Task<PaginationResponse<Notification>> GetAllAsync(int page, int perPage, string userId)
        {
            try 
            {
                var paginationParams = Pagination.BuildPaginationLimit(page, perPage, 32);

                FilterDefinition<Notification> filter = FilterDefinition<Notification>.Empty;

                if (!string.IsNullOrEmpty(userId))
                    filter = Builders<Notification>.Filter.Eq(noti => noti.UserId, userId);

                var sortDefinition = Builders<Notification>.Sort.Descending(noti => noti.CreatedAt);

                var data = await _notificationsCollection.Find(filter)
                                                .Sort(sortDefinition)
                                                .Skip(paginationParams.Skip)
                                                .Limit(paginationParams.PerPage)
                                                .ToListAsync();

                foreach(var noti in data)
                {
                    var user = await _usersCollection.Find(u => u.Id == noti.UserId).FirstOrDefaultAsync();
                    UserNoPassword userNoPassword = (UserNoPassword)user;
                    noti.User = userNoPassword;
                    var evt = await _eventsCollection.Find(evt => evt.Id == noti.EventId).FirstOrDefaultAsync();
                    noti.Event = evt;
                }

                var totalCount = await _notificationsCollection.CountDocumentsAsync(filter);

                var paginationResponse = Pagination.BuildResponsePagination(data, page, perPage, (int)totalCount);

                return paginationResponse;
            }
            catch (Exception) 
            {
                throw;
            }
        }
    }
}
