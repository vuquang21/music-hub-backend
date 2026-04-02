using MediatR;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Search.Commands.AddRecentSearch;

public class AddRecentSearchCommandHandler : IRequestHandler<AddRecentSearchCommand, Unit>
{
    private readonly ISearchHistoryRepository _searchHistoryRepo;
    private readonly IUnitOfWork _unitOfWork;

    public AddRecentSearchCommandHandler(
        ISearchHistoryRepository searchHistoryRepo,
        IUnitOfWork unitOfWork)
    {
        _searchHistoryRepo = searchHistoryRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AddRecentSearchCommand request, CancellationToken ct)
    {
        var existing = await _searchHistoryRepo
            .GetByUserAndTrackAsync(request.UserId, request.TrackId, ct);

        if (existing is not null)
        {
            existing.Touch();
        }
        else
        {
            var entry = SearchHistory.Create(request.UserId, request.TrackId);
            await _searchHistoryRepo.AddAsync(entry, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
