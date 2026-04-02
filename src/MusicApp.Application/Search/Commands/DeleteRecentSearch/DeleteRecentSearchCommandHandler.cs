using MediatR;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Search.Commands.DeleteRecentSearch;

public class DeleteRecentSearchCommandHandler : IRequestHandler<DeleteRecentSearchCommand, Unit>
{
    private readonly ISearchHistoryRepository _searchHistoryRepo;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRecentSearchCommandHandler(
        ISearchHistoryRepository searchHistoryRepo,
        IUnitOfWork unitOfWork)
    {
        _searchHistoryRepo = searchHistoryRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteRecentSearchCommand request, CancellationToken ct)
    {
        var entry = await _searchHistoryRepo
            .GetByIdAndUserAsync(request.SearchHistoryId, request.UserId, ct);

        if (entry is not null)
        {
            _searchHistoryRepo.Remove(entry);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        return Unit.Value;
    }
}
