using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApp.Application.Auth.Commands.Login;
using MusicApp.Application.Auth.Commands.Logout;
using MusicApp.Application.Auth.Commands.RefreshToken;
using MusicApp.Application.Auth.Commands.Register;
using MusicApp.Application.Auth.DTOs;
using MusicApp.Application.Common.DTOs;
using MusicApp.Domain.Exceptions;

namespace MusicApp.API.Controllers;

[AllowAnonymous]
public class AuthController : ApiController
{
    public AuthController(ISender sender) : base(sender) { }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand cmd, CancellationToken ct)
    {
        var result = await _sender.Send(cmd, ct);
        SetRefreshTokenCookie(result.RefreshToken);
        return Created(result with { RefreshToken = null }, "/api/v1/users/me");
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand cmd, CancellationToken ct)
    {
        var result = await _sender.Send(cmd, ct);
        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(result with { RefreshToken = null });
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var token = Request.Cookies["refreshToken"]
            ?? throw new UnauthorizedException("Refresh token not found.");

        var result = await _sender.Send(new RefreshTokenCommand(token), ct);
        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(result with { RefreshToken = null });
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var token = Request.Cookies["refreshToken"] ?? string.Empty;
        await _sender.Send(new LogoutCommand(token), ct);
        DeleteRefreshTokenCookie();
        return NoContent();
    }

    private void SetRefreshTokenCookie(string? token)
    {
        if (string.IsNullOrEmpty(token)) return;
        Response.Cookies.Append("refreshToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            Path = "/api/v1/auth",
        });
    }

    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/v1/auth",
        });
    }
}
