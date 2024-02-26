using HaBuddies.Models;
using HaBuddies.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HaBuddies.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService) =>
            _userService = userService;

        [HttpGet("{id}")]
        public async Task<ActionResult<UserNoPassword>> GetUserById(string id)
        {
            var user = await _userService.GetUserById(id);

            if (user == null)
            {
                return NotFound("User not Found");
            }

            return user;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(User user)
        {
            var token = await _userService.Register(user);

            if (token == null) {
                return Unauthorized();
            }

            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto userDto)
        {
            var token = await _userService.Login(userDto);

            if (token == null) {
                return Unauthorized();
            }

            return Ok(token);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateUser updateUser)
        {
            var user = await _userService.UpdateUser(id, updateUser);

            if (user == null)
            {
                return NotFound("User not Found");
            }

            return Ok(user);
        }
        // [HttpPost]
        // public async Task<IActionResult> Login([FromBody] UserDto userDto)
        // {
        //     var token = await _userService.login(userDto);

        //     if (token == null) {
        //         return Unauthorized();
        //     }

        //     return RedirectToAction("Profile", new { accessToken = token });
        // }

        // [HttpPut]
        // public async Task<IActionResult> Update(string id, [FromBody] UpdateUser updateUser)
        // {
        //     var user = await _userService.updateUser(id, updateUser);

        //     if (user == null)
        //     {
        //         return NotFound("User not found");
        //     }

        //     return RedirectToAction("Profile", new { user });
        // }
    }
}