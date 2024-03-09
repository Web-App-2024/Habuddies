using HaBuddies.DTOs;
using HaBuddies.Models;
using HaBuddies.Services;
using Microsoft.AspNetCore.Mvc;

namespace HaBuddies.Controllers
{
    public class NotificationController : Controller
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService) =>
            _notificationService = notificationService;

        [HttpGet]
        public async Task<IActionResult> LoadNotification()
        {
            try
            {
                UserNoPassword user = HttpContext.Session.Get<UserNoPassword>("user")!;
                string userId = user.Id;
                if (user == null) {
                    return Unauthorized();
                }
                var notifications = await _notificationService.GetAllAsync(userId);
                
                return PartialView("", notifications);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
