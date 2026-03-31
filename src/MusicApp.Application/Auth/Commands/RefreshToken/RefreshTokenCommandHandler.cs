using AutoMapper;
using MediatR;
using MusicApp.Application.Auth.DTOs;
using MusicApp.Application.Common.Interfaces;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepo;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public RefreshTokenCommandHandler(
        IUserRepository userRepo, ITokenService tokenService,
        IUnitOfWork uow, IMapper mapper)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand cmd, CancellationToken ct)
    {
        var user = await _userRepo.GetByRefreshTokenAsync(cmd.Token, ct)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        var existingToken = user.GetActiveRefreshToken(cmd.Token)
            ?? throw new UnauthorizedException("Refresh token expired or revoked.");

        var newRefreshToken = Domain.Entities.RefreshToken.Create(user.Id);
        existingToken.Revoke("Replaced", newRefreshToken.Token);
        user.AddRefreshToken(newRefreshToken);
        _uow.Add(newRefreshToken);
        await _uow.SaveChangesAsync(ct);

        var accessToken = _tokenService.GenerateAccessToken(user);
        return new AuthResponseDto(
            AccessToken: accessToken, ExpiresIn: 900, TokenType: "Bearer",
            User: _mapper.Map<UserDto>(user), RefreshToken: newRefreshToken.Token);
    }
}
