using Microsoft.AspNetCore.Http;

namespace MusicApp.Application.Common.Interfaces;

public interface IStorageService
{
    Task<string> UploadAudioAsync(IFormFile file, CancellationToken ct = default);
    Task<string> UploadImageAsync(IFormFile file, CancellationToken ct = default);
    Task DeleteAsync(string key, CancellationToken ct = default);
    string GetPresignedUrl(string key, int expiryMinutes = 60);
}
