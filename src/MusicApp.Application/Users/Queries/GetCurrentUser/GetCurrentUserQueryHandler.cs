using AutoMapper;
using MediatR;
using MusicApp.Application.Auth.DTOs;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Users.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;

    public GetCurrentUserQueryHandler(IUserRepository userRepo, IMapper mapper)
    { _userRepo = userRepo; _mapper = mapper; }

    public async Task<UserDto> Handle(GetCurrentUserQuery q, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(q.UserId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), q.UserId);
        return _mapper.Map<UserDto>(user);
    }
}
