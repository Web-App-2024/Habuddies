using System.Text;
using HaBuddies.Models;
using HaBuddies.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        public IActionResult MyProfile(UserNoPassword user)
        {
            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Register([FromBody] User user)
        {
            var userExist = await _userService.Register(user);

            if (userExist == null)
            {
                return RedirectToAction(nameof(LoginAndRegister));
            }

            string serializedUser = JsonConvert.SerializeObject(userExist);
            HttpContext.Session.SetString("user", serializedUser);
            Console.WriteLine(HttpContext.Session.GetString("user"));
            return RedirectToAction("Index", "Event");
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromBody] UserDto userDto)
        {
            var userExist = await _userService.Login(userDto);

            if (userExist == null)
            {
                return RedirectToAction(nameof(LoginAndRegister));
            }

            string serializedUser = JsonConvert.SerializeObject(userExist);
            HttpContext.Session.SetString("user", serializedUser);
            Console.WriteLine(HttpContext.Session.GetString("user"));
            return RedirectToAction("Index", "Event");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Console.WriteLine("logout already");
            return RedirectToAction(nameof(LoginAndRegister));
        }

        [HttpPut]
        public async Task<ActionResult> Update(UpdateUser updateUser)
        {
            string id = HttpContext.Session.GetString("user")!;
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(nameof(LoginAndRegister));
            }

            var user = await _userService.UpdateUser(id, updateUser);

            if (user == null)
            {
                return RedirectToAction("LoginAndRegister");
            }
            return RedirectToAction(nameof(MyProfile), new { user });
        }

        public async Task<ActionResult> GetMyProfile()
        {
            string id = HttpContext.Session.GetString("userId")!;

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(nameof(LoginAndRegister));
            }

            var user = await _userService.GetUserById(id);
            return RedirectToAction(nameof(MyProfile), new { user });
        }

        public async Task<ActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserById(id);

            if (user == null)
            {
                return RedirectToAction(nameof(LoginAndRegister));
            }

            ViewBag.UserDetails = user;
            // popup
            return PartialView("UserProfilePartial");
        }
    }
}