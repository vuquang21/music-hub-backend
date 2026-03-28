using System.Security.Claims;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicApp.Application.Common.DTOs;

namespace MusicApp.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public abstract class ApiController : ControllerBase
{
    protected readonly ISender _sender;

    protected ApiController(ISender sender) => _sender = sender;

    protected IActionResult Ok<T>(T data, string? message = null)
        => base.Ok(ApiResponse.Ok(data, message));

    protected IActionResult Created<T>(T data, string location)
        => base.Created(location, ApiResponse.Ok(data));

    protected IActionResult Paged<T>(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
        => base.Ok(ApiResponse.Paged(items, totalCount, page, pageSize));

    protected Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
