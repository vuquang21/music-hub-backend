using AutoMapper;
using MediatR;
using MusicApp.Application.Auth.DTOs;
using MusicApp.Application.Common.Interfaces;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public RegisterCommandHandler(
        IUserRepository userRepo, IPasswordHasher hasher,
        ITokenService tokenService, IUnitOfWork uow, IMapper mapper)
    {
        _userRepo = userRepo;
        _hasher = hasher;
        _tokenService = tokenService;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand cmd, CancellationToken ct)
    {
        var hash = _hasher.Hash(cmd.Password);
        var user = User.Create(cmd.Email, hash, cmd.DisplayName);

        var refreshToken = Domain.Entities.RefreshToken.Create(user.Id);
        user.AddRefreshToken(refreshToken);

        await _userRepo.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        var accessToken = _tokenService.GenerateAccessToken(user);
        return new AuthResponseDto(
            AccessToken: accessToken, ExpiresIn: 900, TokenType: "Bearer",
            User: _mapper.Map<UserDto>(user), RefreshToken: refreshToken.Token);
    }
}
