using HaBuddies.Models;
using MongoDB.Driver;
using AutoMapper;
using HaBuddies.DTOs;
using System.Diagnostics.Tracing;

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
                CalculateTimeUntilMidnight(),
                TimeSpan.FromDays(1)
            );

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateEventDTO, Event>();
            });

            _mapper = config.CreateMapper();
        }

        public async Task<PaginationResponse<Event>> GetAllAsync(int page, int perPage, string category, string userId)
        {
            try 
            {
                var paginationParams = Pagination.BuildPaginationLimit(page, perPage, 32);

                FilterDefinition<Event> filter;
                filter = Builders<Event>.Filter.Eq(evt => evt.IsOpen, true);
                if (!string.IsNullOrEmpty(userId))
                    filter &= Builders<Event>.Filter.Ne(evt => evt.OwnerId, userId);
                if (!string.IsNullOrEmpty(category))
                    filter &= Builders<Event>.Filter.Eq(evt => evt.Category, category);

                var sortDefinition = Builders<Event>.Sort.Descending(evt => evt.CreatedAt);

                var data = await _eventsCollection.Find(filter)
                                                .Sort(sortDefinition)
                                                .Skip(paginationParams.Skip)
                                                .Limit(paginationParams.PerPage)
                                                .ToListAsync();

                foreach(var evt in data)
                {
                    var user = await _usersCollection.Find(u => u.Id == evt.OwnerId).FirstOrDefaultAsync();
                    UserNoPassword userNoPassword = (UserNoPassword)user;
                    evt.Owner = userNoPassword;
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
                Event evt = await _eventsCollection.Find(evt => evt.Id == id).FirstOrDefaultAsync();
                if (evt == null){
                    return evt;
                }

                var projection = Builders<User>.Projection
                    .Exclude(u => u.Password)
                    .Exclude(u => u.JoinedEvent);

                var user = await _usersCollection
                    .Find(u => u.Id == evt.OwnerId)
                    .Project<UserNoPassword>(projection)
                    .FirstOrDefaultAsync();

                evt.Owner = user;

                var subscriberIds = evt.SubscribersId;
                evt.Subscribers = await _usersCollection
                    .Find(u => subscriberIds.Contains(u.Id))
                    .Project<UserNoPassword>(projection)
                    .ToListAsync();

                var QueueIdIds = evt.QueueId;
                evt.Queue = await _usersCollection
                    .Find(u => QueueIdIds.Contains(u.Id))
                    .Project<UserNoPassword>(projection)
                    .ToListAsync();

                return evt;
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<Event> CreateAsync(CreateEventDTO newEventDTO, string userId) 
        {
            try {
                if (newEventDTO.MinAgeRequirement != null || newEventDTO.MaxAgeRequirement != null){
                    if (newEventDTO.MinAgeRequirement == null || newEventDTO.MaxAgeRequirement == null){
                        throw new Exception("Bad Request");
                    }
                }
                if (newEventDTO.GenderRequirement == null){
                    newEventDTO.GenderRequirement = ["Male", "Female", "Others"];
                }
                Event newEvent = _mapper.Map<Event>(newEventDTO);
                newEvent.OwnerId = userId;
                await _eventsCollection.InsertOneAsync(newEvent);
                return newEvent;
            } catch (Exception) {
                throw;
            }
        }

        public async Task<Event> EditAsync(string id, EditEventDTO editedEventDTO, string userId) 
        {
            try {
                var filter = Builders<Event>.Filter.Eq(evt => evt.Id, id);
                var evt = await _eventsCollection.Find(filter).FirstOrDefaultAsync();

                if (evt == null) {
                    throw new Exception("Event Not Found");
                }
                else if (userId != evt.OwnerId) {
                    throw new Exception("Forbidden");
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

        public async Task RemoveAsync(string id, string userId) 
        {
            try {
                var filter = Builders<Event>.Filter.Eq(evt => evt.Id, id);
                var evt = await _eventsCollection.Find(filter).FirstOrDefaultAsync();

                if (evt == null) {
                    throw new Exception("Event Not Found");
                } 
                else if (userId != evt.OwnerId) {
                    throw new Exception("Forbidden");
                }

                await _eventsCollection.DeleteOneAsync(evt => evt.Id == id);

                var updateFilter = Builders<User>.Filter.ElemMatch(u => u.JoinedEvent, id);
                var update = Builders<User>.Update.Pull(u => u.JoinedEvent, id);
                await _usersCollection.UpdateManyAsync(updateFilter, update);
            }
            catch (Exception) {
                throw;
            }
        }

        public async Task SubscribeEvent(string id, string userId)
        {
            try {
                var user = await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                var evt = await GetOneAsync(id);
                var filterUser = Builders<User>.Filter.Eq(u => u.Id, userId);
                

                if (evt == null) {
                    throw new Exception("Event Not Found");
                } 
                else if (userId == evt.OwnerId) {
                    throw new Exception("Forbidden");
                }

                var filter = Builders<Event>.Filter.Eq(evt => evt.Id, id);

                if(user.Age < evt.MinAgeRequirement || user.Age > evt.MaxAgeRequirement){
                    throw new Exception("User Not Suit");
                }
                else if(!evt.GenderRequirement.Contains(user.Gender)){
                    throw new Exception("User Not Suit");
                }

                if (!evt.SubscribersId.Contains(userId) && !evt.QueueId.Contains(userId))
                {
                    if (evt.SubscribersId.Count < evt.EnrollSize){
                        await _eventsCollection.UpdateOneAsync(filter, 
                            Builders<Event>.Update.Push(evt => evt.SubscribersId, userId));
                        await _usersCollection.UpdateOneAsync(filterUser, 
                            Builders<User>.Update.Push(u => u.JoinedEvent, evt.Id));
                    }
                    else if (evt.SubscribersId.Count >= evt.EnrollSize){
                        await _eventsCollection.UpdateOneAsync(filter, 
                            Builders<Event>.Update.Push(evt => evt.QueueId, userId));
                    }
                } 
                else {
                    if(evt.SubscribersId.Contains(userId)){
                        await _eventsCollection.UpdateOneAsync(filter,
                            Builders<Event>.Update.Pull(evt => evt.SubscribersId, userId));
                        await _usersCollection.UpdateOneAsync(filterUser, 
                            Builders<User>.Update.Pull(u => u.JoinedEvent, evt.Id));

                        if (evt.QueueId.Count > 0)
                        {
                            var userFromQueueId = evt.QueueId[0];
                            await _eventsCollection.UpdateOneAsync(filter,
                                Builders<Event>.Update.Push(evt => evt.SubscribersId, userFromQueueId)
                                    .Pull(evt => evt.QueueId, userFromQueueId));
                            await _usersCollection.UpdateOneAsync(u => u.Id == userFromQueueId, 
                                Builders<User>.Update.Push(u => u.JoinedEvent, evt.Id));
                        }
                    }
                    else {
                        await _eventsCollection.UpdateOneAsync(filter, 
                            Builders<Event>.Update.Pull(evt => evt.QueueId, userId));
                    }
                }
            }
            catch (Exception) {
                throw;
            }
        }

        public async Task UpdateEventsAsync()
        {
            var filter = Builders<Event>.Filter.Eq(evt => evt.IsOpen, true);
            var events = await _eventsCollection.Find(filter).ToListAsync();

            foreach (var evt in events)
            {
                if (DateTime.UtcNow > evt.EndDate)
                {
                    evt.IsOpen = false;
                    var updateEvent = Builders<Event>.Update.Set(e => e.IsOpen, false);
                    await _eventsCollection.UpdateOneAsync(e => e.Id == evt.Id, updateEvent);
                    Console.WriteLine($"Close Event Id{evt.Id}");

                    var filterUser = Builders<User>.Filter.In(u => u.Id, evt.SubscribersId);
                    var update = Builders<User>.Update.Push(u => u.JoinedEvent, evt.Id);
                    await _usersCollection.UpdateManyAsync(filterUser, update);
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
