using MusicApp.Domain.Entities;

namespace MusicApp.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
}
