using HaBuddies.Models;
using HaBuddies.Services;
using Microsoft.AspNetCore.Mvc;

namespace HaBuddies.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller {
        private readonly UserService _userService;

        public UserController(UserService userService) {
            _userService = userService;
        }
        [HttpGet("{id}")]
        public async Task<UserNoPassword> getUserById(string id) {
            return await _userService.getUserById(id);
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> register([FromBody] User createUser) {
            var token = await _userService.register(createUser);
            if (token == "This Email already in use.") {
                return Conflict(new { message = "This Email already in use." });
            }
            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UserDto userDto) {
            var token = await _userService.login(userDto);
            if (token == "This email not found.") {
                return BadRequest(new { message = "This Email not found."});
            }
            
            if (token == "Your Email or Password is wrong.") {
                return BadRequest(new { message = "Your Email or Password is wrong."});
            }
            return Ok(new { token = token });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserNoPassword>> updateProfile(string id, [FromBody] UpdateUser updateUser) {
            UserNoPassword user = await _userService.updateUser(id, updateUser);
            return Ok(new { message = "Updated!", user });
        }
    }
}