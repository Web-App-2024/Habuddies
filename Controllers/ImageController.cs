using HaBuddies.Models;
using HaBuddies.Services;
using Microsoft.AspNetCore.Mvc;

namespace HaBuddies.Controllers
{
    public class ImageController(ImageService imageService) : Controller
    {
        private readonly ImageService _imageService = imageService;

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

                var uploadedFileName = await _imageService.UploadImage(file, user.Id);

                if (uploadedFileName == null)
                {
                    return StatusCode(204);
                }

                return RedirectToAction("MyProfile", "User");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500);
            }
        }
    }
}