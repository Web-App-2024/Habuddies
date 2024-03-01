using HaBuddies.Models;
using HaBuddies.Services;
using Microsoft.AspNetCore.Mvc;

namespace HaBuddies.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService) =>
            _userService = userService;

        public IActionResult LoginAndRegister()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult MyProfile(UserNoPassword user)
        {
            return View(user);
        }

        public IActionResult OtherProfile(UserNoPassword user)
        {
            return View(user);
        }

        public async Task<IActionResult> Register([FromBody] User user)
        {
            var id = await _userService.Register(user);

            if (id == null)
            {
                return LoginAndRegister();
            }

            HttpContext.Session.SetString("userId", id);
            // redirect or return to event view
            return View();
        }

        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            var id = await _userService.Login(userDto);

            if (id == null)
            {
                return LoginAndRegister();
            }

            HttpContext.Session.SetString("userId", id);
            // redirect or return to event view
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return LoginAndRegister();
        }

        public async Task<IActionResult> Update(UpdateUser updateUser)
        {
            string id = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(id))
            {
                return LoginAndRegister();
            }

            var user = await _userService.UpdateUser(id, updateUser);

            if (user == null)
            {
                return LoginAndRegister();
            }
            return MyProfile(user);
        }

        public async Task<IActionResult> GetMyProfile()
        {
            string id = HttpContext.Session.GetString("userId");

            if (string.IsNullOrEmpty(id))
            {
                return LoginAndRegister();
            }

            var user = await _userService.GetUserById(id);
            return MyProfile(user);
        }

        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserById(id);

            if (user == null)
            {
                return LoginAndRegister();
            }

            return OtherProfile(user);
        }
    }
}