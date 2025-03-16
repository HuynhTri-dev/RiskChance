using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RiskChance.Utils
{
    public class ImageUtil
    {
        public static async Task<string> SaveAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("Ảnh không hợp lệ.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(image.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Chỉ hỗ trợ các file ảnh (JPG, PNG, GIF).");

            var uploadsFolder = Path.Combine("wwwroot", "upload", "logos");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return $"/upload/logos/{uniqueFileName}"; // Đường dẫn tương đối
        }
    }
}
