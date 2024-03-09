using HaBuddies.Models;
using HaBuddies.Services;
using Microsoft.AspNetCore.Mvc;

namespace HaBuddies.Controllers
{
    public class UploadFileController(UploadImageService uploadImageService) : Controller
    {
        private readonly UploadImageService _uploadImageService = uploadImageService;

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                UserNoPassword user = HttpContext.Session.Get<UserNoPassword>("user")!;
                if (user == null) {
                    return Unauthorized();
                }

                var uploadedFileName = await _uploadImageService.UploadImage(file, user.Id);

                if (uploadedFileName == null)
                {
                    return StatusCode(204);
                }

                return RedirectToAction("MyProfile", "User");
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}