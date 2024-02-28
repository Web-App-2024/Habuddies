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
            User user = await _userCollection.Find(_user => _user.id == id).SingleOrDefaultAsync();
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
                .Set("name", updateUser.name)
                .Set("surname", updateUser.surname)
                .Set("password", updateUser.password); // Note: In a production scenario, you should hash the password

            User user = await _userCollection.FindOneAndUpdateAsync<User>(_user => _user.id == id, update, option);
            UserNoPassword _user = (UserNoPassword)user;
            return _user;
        }

        public async Task<string?> Register(User user)
        {
            var existingUser = await _userCollection.Find(_user => _user.email == user.email).SingleOrDefaultAsync();
            if (existingUser != null)
            {
                return null;
            }

            var newUser = new User
            {
                email = user.email,
                name = user.name,
                surname = user.surname,
                password = user.password
            };

            await _userCollection.InsertOneAsync(newUser);
            return newUser.id;
        }

        public async Task<string?> Login(UserDto user)
        {
            var existingUser = await _userCollection.Find(_user => _user.email == user.email).SingleOrDefaultAsync();

            if (existingUser == null || existingUser.password != user.password)
            {
                // Not found or wrong password
                return null;
            }
            return existingUser.id;
        }
    }
}