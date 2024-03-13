using AutoMapper;
using HaBuddies.DTOs;
using HaBuddies.Models;
using MongoDB.Driver;

namespace HaBuddies.Services
{
    public class UserService
    {
        private readonly MongoService _mongoService;
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<Event> _eventsCollection;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(MongoService mongoService, IConfiguration configuration)
        {
            _mongoService = mongoService;
            _userCollection = _mongoService._userCollection;
             _eventsCollection = _mongoService._eventsCollection;
            _configuration = configuration;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateUserDTO, User>();
            });

            _mapper = config.CreateMapper();
        }

        public async Task<UserNoPassword> GetUserById(string id)
        {
            User user = await _userCollection.Find(_user => _user.Id == id).SingleOrDefaultAsync();
            UserNoPassword userNoPassword = (UserNoPassword)user;
            return userNoPassword;
        }

        public async Task<UserNoPassword> UpdateUser(string id, EditUserDTO editUserDTO)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var userExist = await _userCollection.Find(filter).FirstOrDefaultAsync();

            if (userExist == null) {
                throw new Exception("User Not Found");
            }

            var age = DateTime.Today.Year - editUserDTO.BirthDate.Year;
            editUserDTO.Age = age - (editUserDTO.BirthDate.Date > DateTime.Today.Date.AddYears(-age) ? 1 : 0);

            var update = Builders<User>.Update
                .Set("Name", editUserDTO.Name)
                .Set("Surname", editUserDTO.Surname)
                .Set("Password", editUserDTO.Password)
                .Set("BirthDate", editUserDTO.BirthDate)
                .Set("Age", editUserDTO.Age)
                .Set("Gender", editUserDTO.Gender)
                .Set("Bio", editUserDTO.Bio);

            User user = await _userCollection.FindOneAndUpdateAsync<User>(_user => _user.Id == id, update);
            UserNoPassword _user = (UserNoPassword)user;
            return _user;
        }

        public async Task<UserNoPassword?> Register(CreateUserDTO createUserDTO)
        {
            var existingUser = await _userCollection.Find(_user => _user.Email == createUserDTO.Email).SingleOrDefaultAsync();
            if (existingUser != null)
            {
                return null;
            }

            var age = DateTime.Today.Year - createUserDTO.BirthDate.Year;
            createUserDTO.Age = age - (createUserDTO.BirthDate.Date > DateTime.Today.Date.AddYears(-age) ? 1 : 0);

            User newUser = _mapper.Map<User>(createUserDTO);
            newUser.JoinedEvent = [];

            await _userCollection.InsertOneAsync(newUser);
            UserNoPassword userNoPassword = (UserNoPassword)newUser;
            return userNoPassword;
        }

        public async Task<UserNoPassword?> Login(LoginUserDTO loginUserDTO)
        {
            var existingUser = await _userCollection.Find(_user => _user.Email == loginUserDTO.Email).SingleOrDefaultAsync();

            if (existingUser == null || existingUser.Password != loginUserDTO.Password)
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
                _event => 
                (joinedEventIds!.Contains(_event.Id) || _event.OwnerId == existingUser.Id) &&
                !_event.IsOpen
            ).ToListAsync();

            foreach(var evt in historyEvents)
            {
                var user = await _userCollection.Find(user => user.Id == evt.OwnerId).FirstOrDefaultAsync();
                UserNoPassword userNoPassword = (UserNoPassword)user;
                evt.Owner = userNoPassword;
            }
            
            return historyEvents.ToArray();
        }

        public async Task<MyPostResponse> GetMyPost(string Id)
        {
            var existingUser = await _userCollection.Find(_user => _user.Id == Id).SingleOrDefaultAsync();
            var myEvents = await _eventsCollection.Find(
                _event => _event.OwnerId == existingUser.Id &&
                _event.IsOpen
            ).ToListAsync();

            foreach(var evt in myEvents)
            {
                var user = await _userCollection.Find(user => user.Id == evt.OwnerId).FirstOrDefaultAsync();
                UserNoPassword userNoPassword = (UserNoPassword)user;
                evt.Owner = userNoPassword;
            }

            var joinedEvents = await _eventsCollection.Find(
                _event => 
                _event.SubscribersId.Contains(Id) &&
                _event.IsOpen
            ).ToListAsync();

            foreach (var evt in joinedEvents)
            {
                var user = await _userCollection.Find(user => user.Id == evt.OwnerId).FirstOrDefaultAsync();
                UserNoPassword userNoPassword = (UserNoPassword)user;
                evt.Owner = userNoPassword;
            }
            var response = new MyPostResponse
            {
                Owned = myEvents,
                Joined = joinedEvents
            };
            return response;
        }
    }
}