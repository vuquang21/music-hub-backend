using Microsoft.AspNetCore.Http;
using MusicApp.Application.Common.Interfaces;

namespace MusicApp.Infrastructure.Services;

public class LocalStorageService : IStorageService
{
    private readonly string _basePath;

    public LocalStorageService()
    {
        _basePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> UploadAudioAsync(IFormFile file, CancellationToken ct)
    {
        var key = $"audio/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var fullPath = Path.Combine(_basePath, key);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream, ct);
        return key;
    }

    public async Task<string> UploadImageAsync(IFormFile file, CancellationToken ct)
    {
        var key = $"images/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var fullPath = Path.Combine(_basePath, key);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream, ct);
        return key;
    }

    public Task DeleteAsync(string key, CancellationToken ct)
    {
        var fullPath = Path.Combine(_basePath, key);
        if (File.Exists(fullPath)) File.Delete(fullPath);
        return Task.CompletedTask;
    }

    public string GetPresignedUrl(string key, int expiryMinutes = 60) => $"/storage/{key}";
}
