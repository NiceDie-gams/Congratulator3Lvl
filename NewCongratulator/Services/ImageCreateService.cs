using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NewCongratulator.Services
{
    public class ImageCreateService
    {
        private readonly IWebHostEnvironment _env;

        public ImageCreateService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task<string> ImageCreate(IFormFile imageFile )
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(fileStream);

                return ("/uploads/" + uniqueFileName);
            }
            return "Invalid File";
        }
    }
}
