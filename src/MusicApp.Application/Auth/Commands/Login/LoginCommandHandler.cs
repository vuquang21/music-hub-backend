using AutoMapper;
using MediatR;
using MusicApp.Application.Auth.DTOs;
using MusicApp.Application.Common.Interfaces;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public LoginCommandHandler(
        IUserRepository userRepo, IPasswordHasher hasher,
        ITokenService tokenService, IUnitOfWork uow, IMapper mapper)
    {
        _userRepo = userRepo;
        _hasher = hasher;
        _tokenService = tokenService;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var user = await _userRepo.GetByEmailAsync(cmd.Email.ToLowerInvariant(), ct)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!_hasher.Verify(cmd.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        if (!user.IsActive)
            throw new ForbiddenException("Account is deactivated.");

        var refreshToken = Domain.Entities.RefreshToken.Create(user.Id);
        user.AddRefreshToken(refreshToken);
        await _uow.SaveChangesAsync(ct);

        var accessToken = _tokenService.GenerateAccessToken(user);
        return new AuthResponseDto(
            AccessToken: accessToken, ExpiresIn: 900, TokenType: "Bearer",
            User: _mapper.Map<UserDto>(user), RefreshToken: refreshToken.Token);
    }
}
