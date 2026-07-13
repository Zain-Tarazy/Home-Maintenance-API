
using HomeMaintenanceAPI.Application.Interfaces.Services;

namespace HomeMaintenanceAPI.Application.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private static readonly string[] AllowedExtensions =
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        private readonly IWebHostEnvironment _environment;

        public LocalFileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveImageAsync(
                Stream fileStream,
                string originalFileName,
                string contentType,
                long fileSize,
                string folderName,
                CancellationToken cancellationToken = default)
        {
            if (fileSize <= 0)
                throw new InvalidOperationException("Image file is empty.");

            if (fileSize > MaxFileSize)
                throw new InvalidOperationException("Maximum image size is 5 MB.");

            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
                throw new InvalidOperationException("Only JPG, JPEG, PNG, and WEBP images are allowed.");

            if (string.IsNullOrWhiteSpace(contentType) ||
                !contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid image content type.");
            }

            var webRootPath = _environment.WebRootPath;

            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            var year = DateTime.UtcNow.ToString("yyyy");
            var month = DateTime.UtcNow.ToString("MM");

            var uploadFolder = Path.Combine(
                webRootPath,
                "uploads",
                folderName,
                year,
                month);

            Directory.CreateDirectory(uploadFolder);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(uploadFolder, fileName);

            await using var outputStream = new FileStream(fullPath, FileMode.CreateNew);
            await fileStream.CopyToAsync(outputStream, cancellationToken);

            return $"/uploads/{folderName}/{year}/{month}/{fileName}";
        }

        public void DeleteFile(string? fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return;

            var webRootPath = _environment.WebRootPath;

            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            var relativePath = fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.GetFullPath(Path.Combine(webRootPath, relativePath));

            var safeRootPath = Path.GetFullPath(webRootPath);

            if (!fullPath.StartsWith(safeRootPath))
                return;

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
