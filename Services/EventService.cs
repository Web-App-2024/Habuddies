using System;
using System.Threading;
using System.Threading.Tasks;
using HaBuddies.Models;
using MongoDB.Driver;

namespace HaBuddies.Services
{
    public class EventService
    {
        private readonly MongoService _mongoService;
        private readonly IMongoCollection<Event> _eventsCollection;
        private readonly Timer _timer;

        public EventService(MongoService mongoService)
        {
            _mongoService = mongoService;
            _eventsCollection = _mongoService._eventsCollection;
            _timer = new Timer(async state => await UpdateEventsAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
        }

        public async Task<PaginationResponse<Event>> GetAllAsync(int page, int perPage, string category)
        {
            var paginationParams = Pagination.BuildPaginationLimit(page, perPage, 32);

            FilterDefinition<Event> filter;
            if (!string.IsNullOrEmpty(category))
            {
                filter = Builders<Event>.Filter.Eq(evt => evt.category, category);
            }
            else
            {
                filter = Builders<Event>.Filter.Empty;
            }

            var data = await _eventsCollection.Find(filter)
                                              .Skip(paginationParams.Skip)
                                              .Limit(paginationParams.PerPage)
                                              .ToListAsync();
            var totalCount = await _eventsCollection.CountDocumentsAsync(filter);

            var paginationResponse = Pagination.BuildResponsePagination(data, page, perPage, (int)totalCount);

            return paginationResponse;
        }

        public async Task<Event?> GetOneAsync(string id) =>
            await _eventsCollection.Find(evt => evt.id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Event newEvent) =>
            await _eventsCollection.InsertOneAsync(newEvent);

        public async Task UpdateAsync(string id, Event updatedEvent) =>
            await _eventsCollection.ReplaceOneAsync(evt => evt.id == id, updatedEvent);

        public async Task RemoveAsync(string id) =>
            await _eventsCollection.DeleteOneAsync(evt => evt.id == id);

        public async Task UpdateEventsAsync()
        {
            // Logic to update events asynchronously
        }
    }
}
