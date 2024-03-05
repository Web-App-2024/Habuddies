using HaBuddies.Models;
using MongoDB.Driver;
using AutoMapper;
using HaBuddies.DTOs;

namespace HaBuddies.Services
{
    public class EventService
    {
        private readonly MongoService _mongoService;
        private readonly IMongoCollection<Event> _eventsCollection;
        private readonly IMongoCollection<User> _usersCollection;
        private readonly Timer _timer;
        private readonly IMapper _mapper;

        public EventService(MongoService mongoService)
        {
            _mongoService = mongoService;
            _eventsCollection = _mongoService._eventsCollection;
            _usersCollection = _mongoService._userCollection;

            TimeSpan timeUntilMidnight = CalculateTimeUntilMidnight();
            _timer = new Timer(
                async state => await UpdateEventsAsync(), 
                null, 
                timeUntilMidnight,
                TimeSpan.FromDays(1)
            );

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateEventDTO, Event>();
            });

            _mapper = config.CreateMapper();
        }

        public async Task<PaginationResponse<Event>> GetAllAsync(int page, int perPage, string category)
        {
            try 
            {
                var paginationParams = Pagination.BuildPaginationLimit(page, perPage, 32);

                FilterDefinition<Event> filter;
                if (!string.IsNullOrEmpty(category))
                {
                    filter = Builders<Event>.Filter.Eq(evt => evt.Category, category);
                }
                else
                {
                    filter = Builders<Event>.Filter.Empty;
                }

                var data = await _eventsCollection.Find(filter)
                                                .Skip(paginationParams.Skip)
                                                .Limit(paginationParams.PerPage)
                                                .ToListAsync();

                foreach(var evt in data)
                {
                    evt.Owner = await _usersCollection.Find(u => u.Id == evt.OwnerId).FirstOrDefaultAsync();
                }

                var totalCount = await _eventsCollection.CountDocumentsAsync(filter);

                var paginationResponse = Pagination.BuildResponsePagination(data, page, perPage, (int)totalCount);

                return paginationResponse;
            }
            catch (Exception) 
            {
                throw;
            }
        }


        public async Task<Event?> GetOneAsync(string id) 
        {
            try {
                return await _eventsCollection.Find(evt => evt.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception) {
                throw;
            }
        }
        public async Task<Event> CreateAsync(CreateEventDTO newEventDTO, string userId) 
        {
            try {
                Event newEvent = _mapper.Map<Event>(newEventDTO);
                newEvent.OwnerId = userId;
                await _eventsCollection.InsertOneAsync(newEvent);
                return newEvent;
            } catch (Exception) {
                throw;
            }
        }
        public async Task<Event> EditAsync(string id, EditEventDTO editedEventDTO) 
        {
            try {
                var filter = Builders<Event>.Filter.Eq(evt => evt.Id, id);
                var evt = await _eventsCollection.Find(filter).FirstOrDefaultAsync();

                if (evt == null) {
                    throw new Exception("Event Not Found");
                }

                foreach (var property in typeof(EditEventDTO).GetProperties())
                {
                    var value = property.GetValue(editedEventDTO);
                    if (value != null)
                    {
                        typeof(Event).GetProperty(property.Name)?.SetValue(evt, value);
                    }
                }

                await _eventsCollection.ReplaceOneAsync(filter, evt);

                return evt;
            }
            catch (Exception) {
                throw;
            }
        }

        public async Task RemoveAsync(string id) 
        {
            try {
                var filter = Builders<Event>.Filter.Eq(evt => evt.Id, id);
                var evt = await _eventsCollection.Find(filter).FirstOrDefaultAsync();

                if (evt == null) {
                    throw new Exception("Event Not Found");
                }

                await _eventsCollection.DeleteOneAsync(evt => evt.Id == id);
            }
            catch (Exception) {
                throw;
            }
        }

        public async Task SubscribeEvent(string id, string userId)
        {
            try {
                var evt = await GetOneAsync(id) ?? throw new Exception("Event not found.");
                var filter = Builders<Event>.Filter.Eq(evt => evt.Id, id);
                if (!evt.Subscribers.Contains(userId) && !evt.Queue.Contains(userId))
                {
                    if (evt.Subscribers.Count <= evt.EnrollSize){
                        await _eventsCollection.UpdateOneAsync(filter, 
                            Builders<Event>.Update.Push(evt => evt.Subscribers, userId));
                    }
                    else if (evt.Subscribers.Count >= evt.EnrollSize){
                        await _eventsCollection.UpdateOneAsync(filter, 
                            Builders<Event>.Update.Push(evt => evt.Queue, userId));
                    }
                } 
                else {
                    if(evt.Subscribers.Contains(userId)){
                        await _eventsCollection.UpdateOneAsync(filter,
                            Builders<Event>.Update.Pull(evt => evt.Subscribers, userId));

                        if (evt.Queue.Count > 0)
                        {
                            var userFromQueue = evt.Queue[0];
                            await _eventsCollection.UpdateOneAsync(filter,
                                Builders<Event>.Update.Push(evt => evt.Subscribers, userFromQueue)
                                    .Pull(evt => evt.Queue, userFromQueue));
                        }
                    }
                    else {
                        await _eventsCollection.UpdateOneAsync(filter, 
                            Builders<Event>.Update.Pull(evt => evt.Queue, userId));
                    }
                }
            }
            catch (Exception) {
                throw;
            }
        }

        public async Task UpdateEventsAsync()
        {
            var filter = Builders<Event>.Filter.Empty;
            var events = await _eventsCollection.Find(filter).ToListAsync();

            foreach (var ev in events)
            {
                if (DateTime.UtcNow > ev.EndDate)
                {
                    ev.IsOpen = false;
                    var update = Builders<Event>.Update.Set(e => e.IsOpen, false);
                    await _eventsCollection.UpdateOneAsync(e => e.Id == ev.Id, update);
                }
            }
        }

        private TimeSpan CalculateTimeUntilMidnight()
        {
            DateTime now = DateTime.UtcNow;
            DateTime midnightToday = now.Date.AddDays(1);
            TimeSpan timeUntilMidnight = midnightToday - now;
            return timeUntilMidnight;
        }
    }
}
