using HaBuddies.Models;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;

namespace HaBuddies.Services
{
    public class UserService
    {
        private readonly MongoService _mongoService;
        private readonly IMongoCollection<User> _userCollection;
        private readonly IConfiguration _configuration;

        public UserService(MongoService mongoService, IConfiguration configuration)
        {
            _mongoService = mongoService;
            _userCollection = _mongoService._userCollection;
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

        public async Task<string?> Register(User user)
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

            await _userCollection.InsertOneAsync(newUser);
            return newUser.Id;
        }

        public async Task<string?> Login(UserDto user)
        {
            var existingUser = await _userCollection.Find(_user => _user.Email == user.Email).SingleOrDefaultAsync();

            if (existingUser == null || existingUser.Password != user.Password)
            {
                return null;
            }
            return existingUser.Id;
        }
    }
}