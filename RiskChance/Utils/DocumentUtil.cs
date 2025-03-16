namespace RiskChance.Utils
{
    public class DocumentUtil
    {
        private async Task<string> SaveAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File không hợp lệ.");
            }

            // Chỉ cho phép các định dạng PDF, DOC, DOCX
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Chỉ hỗ trợ các file có định dạng PDF, DOC, DOCX.");
            }

            // Tạo thư mục nếu chưa có
            var uploadsFolder = Path.Combine("wwwroot", "upload", "documents");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Đổi tên file để tránh trùng
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Lưu file vào thư mục
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Trả về đường dẫn file để lưu vào DB (đường dẫn tương đối)
            return $"/upload/documents/{uniqueFileName}";
        }

    }
}
