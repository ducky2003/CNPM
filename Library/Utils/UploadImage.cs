namespace Library.Utils
{
    public static class UploadImage
    {
        // Upload multiple images into default folder "/uploads/images"
        public static string? UploadSingleImage(IFormFile? file)
        {

            // Kiểm tra file có null hoặc trống không
            if (file == null || file.Length == 0)
            {
                return null;
            }

            // Tạo tên file mới bằng cách sử dụng Guid
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            // Đường dẫn tuyệt đối nơi file sẽ được lưu
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "images", fileName);

            // Tạo thư mục nếu chưa tồn tại
            var folderPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Sao chép file vào đường dẫn
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Trả về đường dẫn tương đối để lưu vào cơ sở dữ liệu
            // Ví dụ: /uploads/images/fileName
            var relativePath = Path.Combine("/Uploads", "images", fileName).Replace("\\", "/");

            return relativePath.ToLowerInvariant();

        }
        // Upload single image with custom folder name
        public static string? UploadSingleImage(IFormFile? file, string? folderName)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }
            string storeFolder = folderName ?? "images";
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            // Check if folder exists in wwwroot/uploads/storeFolder
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", storeFolder);
            // If folder doesn't exist, create it in wwwroot/uploads/storeFolder
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", storeFolder, fileName);
            // Copy file to path
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            // Example: /uploads/storeFolder/fileName
            // Return path to save in database
            return path.Substring(path.IndexOf("/Uploads")).ToLowerInvariant();
        }
    }
}