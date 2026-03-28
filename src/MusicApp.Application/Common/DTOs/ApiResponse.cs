namespace MusicApp.Application.Common.DTOs;

public class ApiResponse
{
    public bool Success { get; set; }
    public object? Data { get; set; }
    public string? Message { get; set; }
    public object? Errors { get; set; }
    public object? Meta { get; set; }

    public static ApiResponse Ok(object? data = null, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse Fail(string message, object? errors = null)
        => new() { Success = false, Message = message, Errors = errors };

    public static ApiResponse Paged<T>(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
        => new()
        {
            Success = true,
            Data = items,
            Meta = new
            {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        };
}
