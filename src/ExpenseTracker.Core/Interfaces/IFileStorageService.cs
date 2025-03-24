namespace ExpenseTracker.Core.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType);
    Task<(Stream FileStream, string ContentType)> GetFileAsync(string filePath);
    Task DeleteFileAsync(string filePath);
}
