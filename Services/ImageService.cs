namespace HaBuddies.Services
{
    public class UploadImageService
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        public async Task<string?> UploadImage(IFormFile file, string userId)
        {

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (file == null || file.Length == 0 || !_allowedExtensions.Contains(fileExtension))
            {
                return null;
            }

            var folder = Path.Combine("wwwroot", "uploadFiles");
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), folder);
            
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            
            var uniqueFileName = $"{userId}{fileExtension}";
            var filePath = Path.Combine(folder, uniqueFileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueFileName;
        }
    }
}