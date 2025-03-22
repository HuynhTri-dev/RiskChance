namespace RiskChance.Utils
{
    public class DocumentUtil
    {
        public static async Task<string> SaveAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("Ảnh không hợp lệ.");

            var allowedExtensions = new[] { ".doc", ".pdf", ".docx" };
            var extension = Path.GetExtension(image.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Chỉ hỗ trợ các file doc, pdf, docx");

            var uploadsFolder = Path.Combine("wwwroot", "upload", "documents");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return $"/upload/documents/{uniqueFileName}"; // Đường dẫn tương đối
        }
    }
}
