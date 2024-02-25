using System.Security.Claims;
using HaBuddies.Models;
using MongoDB.Driver;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace HaBuddies.Services {
    public class UserService {
        private MongoService _mongoService;
        private IMongoCollection<User> _userCollection;
        private readonly IConfiguration _configuration;
        public UserService(MongoService mongoService, IConfiguration configuration) {
            _mongoService = mongoService;
            _userCollection = _mongoService._userCollection;
            _configuration = configuration;
        }
        private string createToken(User user) {
            string id = "";
            if (user.id != null) {
                id = user.id;
            }
            List<Claim> claims = new List<Claim>{
                new Claim("name", user.name),
                new Claim("id", id)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:JWTSecretKey").Value
            ));

            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credential
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public async Task<UserNoPassword> getUserById(string id) {
            User user = await _userCollection.Find(_user => _user.id == id).SingleAsync();
            UserNoPassword userNoPassword = (UserNoPassword)user;
            return userNoPassword;
        }

        public async Task<UserNoPassword> updateUser(string id, UpdateUser updateUser) {
            var option = new FindOneAndUpdateOptions<User, User> {
                IsUpsert = false,
                ReturnDocument = ReturnDocument.After
            };

            var update = Builders<User>.Update.Set("name", updateUser.name).Set("surname", updateUser.surname).Set("password", updateUser.password);
            User user = await _userCollection.FindOneAndUpdateAsync<User>(_user => _user.id == id, update, option);
            UserNoPassword _user = (UserNoPassword)user;
            return _user;
        }

        public async Task<string> register(User user) {
            var _existUser = await _userCollection.Find(_user => _user.email == user.id).SingleOrDefaultAsync();
            if (_existUser != null) {
                return "This Email already in use.";
            }

            var _user = new User();
            _user.email = user.email;
            _user.name = user.name;
            _user.surname = user.surname;
            _user.password = user.password;

            await _userCollection.InsertOneAsync(_user);
            return this.createToken(_user);
        }

        public async Task<string> login(UserDto user) {
            var _user = await _userCollection.Find(_user => _user.email == user.email).SingleOrDefaultAsync();
            if (_user == null) {
                return "This email not found.";
            }

            if (_user.password == user.password) {
                return this.createToken(_user);
            }
            return "Your Email or Password is wrong.";
        }
    }
}