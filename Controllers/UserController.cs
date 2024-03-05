using System.Text;
using HaBuddies.Models;
using HaBuddies.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.WebEncoders.Testing;
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
            UserNoPassword userExist = HttpContext.Session.Get<UserNoPassword>("user")!;

            if (userExist == null)
            {
                return RedirectToAction(nameof(LoginAndRegister));
            }

            var events = await _userService.GetMyPost(userExist.Id!);
            return View(events);
        }

        public async Task<IActionResult> History()
        {
            UserNoPassword userExist = HttpContext.Session.Get<UserNoPassword>("user")!;

            if (userExist == null)
            {
                return RedirectToAction(nameof(LoginAndRegister));
            }

            var events = await _userService.GetHistory(userExist.Id!);
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

            HttpContext.Session.Set("user", userExist);
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

            HttpContext.Session.Set("user", userExist); 
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
           UserNoPassword userExist = HttpContext.Session.Get<UserNoPassword>("user")!;

            if (userExist == null)
            {
                return RedirectToAction(nameof(LoginAndRegister));
            }

            var user = await _userService.UpdateUser(userExist.Id!, updateUser);

            if (user == null)
            {
                return RedirectToAction("LoginAndRegister");
            }

            return RedirectToAction(nameof(MyProfile), new { user });
        }

        public async Task<IActionResult> MyProfile()
        {
            UserNoPassword userExist = HttpContext.Session.Get<UserNoPassword>("user")!;

            if (userExist == null)
            {
                return RedirectToAction(nameof(LoginAndRegister));
            }

            var user = await _userService.GetUserById(userExist.Id!);
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