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

        public async Task<IActionResult> MyPost()
        {
            string userExist = HttpContext.Session.GetString("user")!;

            if (string.IsNullOrEmpty(userExist))
            {
                return RedirectToAction(nameof(LoginAndRegister));
            }

            dynamic userObject = JsonConvert.DeserializeObject(userExist)!;
            string userId = userObject.Id;
            var events = await _userService.GetMyPost(userId);
            return View(events);
        }

        public async Task<IActionResult> History()
        {
            string userExist = HttpContext.Session.GetString("user")!;

            if (string.IsNullOrEmpty(userExist))
            {
                return RedirectToAction(nameof(LoginAndRegister));
            }

            dynamic userObject = JsonConvert.DeserializeObject(userExist)!;
            string userId = userObject.Id;
            var events = await _userService.GetHistory(userId);
            return View(events);
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
           string userExist = HttpContext.Session.GetString("user")!;

            if (string.IsNullOrEmpty(userExist))
            {
                return RedirectToAction(nameof(LoginAndRegister));
            }

            dynamic userObject = JsonConvert.DeserializeObject(userExist)!;
            string userId = userObject.Id;

            var user = await _userService.UpdateUser(userId, updateUser);

            if (user == null)
            {
                return RedirectToAction("LoginAndRegister");
            }

            return RedirectToAction(nameof(MyProfile), new { user });
        }

        public async Task<IActionResult> MyProfile()
        {
            string userExist = HttpContext.Session.GetString("user")!;

            if (string.IsNullOrEmpty(userExist))
            {
                return RedirectToAction(nameof(LoginAndRegister));
            }
            dynamic userObject = JsonConvert.DeserializeObject(userExist)!;
            string userId = userObject.Id;

            var user = await _userService.GetUserById(userId);
            return View(user);
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