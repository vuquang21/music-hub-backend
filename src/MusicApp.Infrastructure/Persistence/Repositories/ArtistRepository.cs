using Microsoft.EntityFrameworkCore;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Infrastructure.Persistence.Repositories;

public class ArtistRepository : IArtistRepository
{
    private readonly AppDbContext _context;
    public ArtistRepository(AppDbContext context) => _context = context;

    public async Task<Artist?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _context.Artists.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<Artist?> GetByUserIdAsync(Guid userId, CancellationToken ct)
        => await _context.Artists.FirstOrDefaultAsync(a => a.UserId == userId, ct);

    public async Task AddAsync(Artist artist, CancellationToken ct) => await _context.Artists.AddAsync(artist, ct);
    public void Update(Artist artist) => _context.Artists.Update(artist);
}
