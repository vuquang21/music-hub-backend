using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MusicApp.Application.Common.Interfaces;
using MusicApp.Domain.Enums;
using MusicApp.Domain.Exceptions;

namespace MusicApp.Infrastructure.Auth;

public class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor) => _accessor = accessor;

    public Guid Id => Guid.Parse(_accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedException("User not authenticated."));

    public string Email => _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email)
        ?? throw new UnauthorizedException("User not authenticated.");

    public UserRole Role => Enum.Parse<UserRole>(
        _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) ?? "User");

    public bool IsAuthenticated => _accessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
