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
        public async Task<IActionResult> LoadOwnerNotification(int page = 1, int perPage = 10)
        {
            try
            {
                UserNoPassword user = HttpContext.Session.Get<UserNoPassword>("user")!;
                string userId = user.Id;
                if (user == null) {
                    return Unauthorized();
                }
                var paginationResponse = await _notificationService.GetAllAsync(page, perPage, userId, true);

                if (paginationResponse.Data.Count <= 0 && paginationResponse.PrevPage != null) {
                    return StatusCode(204);
                }
                
                return PartialView("", paginationResponse);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadJoinNotification(int page = 1, int perPage = 10)
        {
            try
            {
                UserNoPassword user = HttpContext.Session.Get<UserNoPassword>("user")!;
                string userId = user.Id;
                if (user == null) {
                    return Unauthorized();
                }
                var paginationResponse = await _notificationService.GetAllAsync(page, perPage, userId, false);

                if (paginationResponse.Data.Count <= 0 && paginationResponse.PrevPage != null) {
                    return StatusCode(204);
                }
                
                return PartialView("", paginationResponse);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
