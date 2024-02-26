using System.Security.Claims;
using HaBuddies.Models;
using MongoDB.Driver;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

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

    private string CreateToken(User user)
    {
        string id = user.id ?? "";
        List<Claim> claims = new List<Claim>
        {
            new Claim("name", user.name),
            new Claim("surname", user.surname),
            new Claim("userID", id)
        };

        string jwtSecretKey = _configuration.GetSection("AppSettings:JWTSecretKey").Value;

        if (string.IsNullOrEmpty(jwtSecretKey))
        {
            // Handle the case where the key is null or empty, for example, by providing a default key.
            // In a production scenario, you should handle this more securely.
            jwtSecretKey = "defaultKey";
        }

        // Ensure that the key has at least 512 bits (64 bytes) for HMAC-SHA512
        var keyBytes = Encoding.UTF8.GetBytes(jwtSecretKey);

        // Pad or truncate the key to meet the required size
        Array.Resize(ref keyBytes, 64);

        var key = new SymmetricSecurityKey(keyBytes);

        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: cred
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
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
        return CreateToken(newUser);
    }

    public async Task<string?> Login(UserDto user)
    {
        var existingUser = await _userCollection.Find(_user => _user.email == user.email).SingleOrDefaultAsync();
        if (existingUser == null || existingUser.password != user.password)
        {
            return null;
        }

        return CreateToken(existingUser);
    }
}

}