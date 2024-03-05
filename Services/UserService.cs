using HaBuddies.Models;
using MongoDB.Driver;

namespace HaBuddies.Services
{
    public class UserService
    {
        private readonly MongoService _mongoService;
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<Event> _eventsCollection;
        private readonly IConfiguration _configuration;

        public UserService(MongoService mongoService, IConfiguration configuration)
        {
            _mongoService = mongoService;
            _userCollection = _mongoService._userCollection;
             _eventsCollection = _mongoService._eventsCollection;
            _configuration = configuration;
        }

        public async Task<UserNoPassword> GetUserById(string id)
        {
            User user = await _userCollection.Find(_user => _user.Id == id).SingleOrDefaultAsync();
            UserNoPassword userNoPassword = (UserNoPassword)user;
            return userNoPassword;
        }

        public async Task<UserNoPassword> UpdateUser(string id, UpdateUser updateUser)
        {
            var option = new FindOneAndUpdateOptions<User, User>
            {
                IsUpsert = false,
                ReturnDocument = ReturnDocument.After
            };

            var update = Builders<User>.Update
                .Set("Name", updateUser.Name)
                .Set("Surname", updateUser.Surname)
                .Set("Password", updateUser.Password)
                .Set("Age", updateUser.Age)
                .Set("Gender", updateUser.Gender)
                .Set("Bio", updateUser.Bio);

            User user = await _userCollection.FindOneAndUpdateAsync<User>(_user => _user.Id == id, update, option);
            UserNoPassword _user = (UserNoPassword)user;
            return _user;
        }

        public async Task<UserNoPassword?> Register(User user)
        {
            var existingUser = await _userCollection.Find(_user => _user.Email == user.Email).SingleOrDefaultAsync();
            if (existingUser != null)
            {
                return null;
            }

            var newUser = new User
            {
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Password = user.Password,
                Age = user.Age,
                Gender = user.Gender,
                Bio = user.Bio,
                JoinedEvent = []
            };

            UserNoPassword userNoPassword = (UserNoPassword)newUser;
            await _userCollection.InsertOneAsync(newUser);
            return userNoPassword;
        }

        public async Task<UserNoPassword?> Login(UserDto user)
        {
            var existingUser = await _userCollection.Find(_user => _user.Email == user.Email).SingleOrDefaultAsync();

            if (existingUser == null || existingUser.Password != user.Password)
            {
                return null;
            }
            UserNoPassword userNoPassword = (UserNoPassword)existingUser;
            return userNoPassword;
        }

        public async Task<Event[]> GetHistory(string Id)
        {
            var existingUser = await _userCollection.Find(_user => _user.Id == Id).SingleOrDefaultAsync();
            var joinedEventIds = existingUser.JoinedEvent;

            var historyEvents = await _eventsCollection.Find(
                _event => joinedEventIds!.Contains(_event.Id) && (!_event.IsOpen)
            ).ToListAsync();

            foreach(var evt in historyEvents)
            {
                var user = await _userCollection.Find(user => user.Id == evt.OwnerId).FirstOrDefaultAsync();
                UserNoPassword userNoPassword = (UserNoPassword)user;
                evt.Owner = userNoPassword;
            }
            
            return historyEvents.ToArray();
        }

        public async Task<Event[]> GetMyPost(string Id)
        {
            var existingUser = await _userCollection.Find(_user => _user.Id == Id).SingleOrDefaultAsync();
            var myEvents = await _eventsCollection.Find(
                _event => _event.OwnerId == existingUser.Id
            ).ToListAsync();

            foreach(var evt in myEvents)
            {
                var user = await _userCollection.Find(user => user.Id == evt.OwnerId).FirstOrDefaultAsync();
                UserNoPassword userNoPassword = (UserNoPassword)user;
                evt.Owner = userNoPassword;
            }

            return myEvents.ToArray();
        }
    }
}