using HaBuddies.Models;
using HaBuddies.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HaBuddies.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService) =>
            _userService = userService;

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] User user)
        {
            var id = await _userService.Register(user);

            if (id == null)
            {
                return Unauthorized();
            }

            HttpContext.Session.SetString("userId", id);
            // return RedirectToAction();
            return Ok(id);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            var id = await _userService.Login(userDto);

            if (id == null)
            {
                return Unauthorized();
            }

            HttpContext.Session.SetString("userId", id);
            // return RedirectToAction();
            return Ok("Login Success!");
        }

        // [Authorize]
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok("Logout Success!");
        }

        // [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update(UpdateUser updateUser)
        {
            string id = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User not authenticated");
            }

            var user = await _userService.UpdateUser(id, updateUser);

            if (user == null)
            {
                return NotFound("User not Found");
            }
            // return RedirectToAction();
            return Ok(user);
        }

        // [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserNoPassword>> GetProfile()
        {
            string id = HttpContext.Session.GetString("userId");

            if (string.IsNullOrEmpty(id)) {
                return BadRequest("User not authenticated");
            }

            var user = await _userService.GetUserById(id);

            if (user == null)
            {
                return NotFound("User not Found");
            }
            // return RedirectToAction();
            return Ok(user);
        }

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
    }
}