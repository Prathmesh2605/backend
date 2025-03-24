using ExpenseTracker.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ExpenseTracker.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadDirectory;

    public LocalFileStorageService(IConfiguration configuration)
    {
        _uploadDirectory = configuration["FileStorage:UploadDirectory"] 
            ?? Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        
        if (!Directory.Exists(_uploadDirectory))
        {
            Directory.CreateDirectory(_uploadDirectory);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(_uploadDirectory, uniqueFileName);

        using var fileStreamWriter = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fileStreamWriter);

        return uniqueFileName;
    }

    public async Task<(Stream FileStream, string ContentType)> GetFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_uploadDirectory, filePath);
        
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Receipt file not found", filePath);
        }

        var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        var contentType = GetContentType(Path.GetExtension(filePath));

        return (fileStream, contentType);
    }

    public Task DeleteFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_uploadDirectory, filePath);
        
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    private static string GetContentType(string fileExtension)
    {
        return fileExtension.ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };
    }
}
