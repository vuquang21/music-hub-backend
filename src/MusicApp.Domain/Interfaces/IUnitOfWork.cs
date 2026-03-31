namespace MusicApp.Domain.Interfaces;

public interface IUnitOfWork
{
    /// <summary>
    /// Registers a new entity so that it will be inserted on the next <see cref="SaveChangesAsync"/> call.
    /// Use this when adding a new child entity to an already-tracked aggregate,
    /// because navigation-based change detection may incorrectly infer Modified state
    /// for entities with client-generated keys.
    /// </summary>
    void Add<T>(T entity) where T : class;

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
