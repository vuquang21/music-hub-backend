using AutoMapper;
using MediatR;
using MusicApp.Application.Common.Interfaces;
using MusicApp.Application.Home.DTOs;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Home.Queries.GetHome;

public class GetHomeQueryHandler : IRequestHandler<GetHomeQuery, HomeDto>
{
    private readonly IHomeRepository _homeRepo;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetHomeQueryHandler(
        IHomeRepository homeRepo,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _homeRepo = homeRepo;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<HomeDto> Handle(GetHomeQuery request, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.SectionLimit, 1, 20);
        var userId = _currentUser.IsAuthenticated ? _currentUser.Id : (Guid?)null;

        // NOTE: EF Core's DbContext is NOT thread-safe.
        // Sections are fetched sequentially to avoid concurrent-operation exceptions.
        // The repository queries are individually optimised — total latency remains acceptable.
        var trending = await _homeRepo.GetTrendingTracksAsync(limit, cancellationToken);
        var forYou = await _homeRepo.GetForYouTracksAsync(userId, limit, cancellationToken);
        var discovery = await _homeRepo.GetDiscoveryWeeklyTracksAsync(userId, limit, cancellationToken);
        var podcasts = await _homeRepo.GetFeaturedPodcastsAsync(limit, cancellationToken);

        return new HomeDto(
            Trending: _mapper.Map<IReadOnlyList<HomeSectionTrackDto>>(trending),
            ForYou: _mapper.Map<IReadOnlyList<HomeSectionTrackDto>>(forYou),
            DiscoveryWeekly: _mapper.Map<IReadOnlyList<HomeSectionTrackDto>>(discovery),
            Podcasts: _mapper.Map<IReadOnlyList<HomePodcastDto>>(podcasts)
        );
    }
}
