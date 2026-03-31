using MediatR;
using MusicApp.Domain.Common;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly IPublisher _publisher;

    public UnitOfWork(AppDbContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public void Add<T>(T entity) where T : class
        => _context.Add(entity);

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        var entities = _context.ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var result = await _context.SaveChangesAsync(ct);

        foreach (var entity in entities)
        {
            foreach (var domainEvent in entity.DomainEvents)
                await _publisher.Publish(domainEvent, ct);
            entity.ClearDomainEvents();
        }

        return result;
    }
}
