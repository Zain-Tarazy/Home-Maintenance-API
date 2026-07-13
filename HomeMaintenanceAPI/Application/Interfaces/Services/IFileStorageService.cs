namespace HomeMaintenanceAPI.Application.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveImageAsync(
            Stream fileStream,
            string originalFileName,
            string contentType,
            long fileSize,
            string folderName,
            CancellationToken cancellationToken = default);

        void DeleteFile(string? fileUrl);
    }
}

