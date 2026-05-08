using System.Linq.Expressions;
using FuelGuard.Shared.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FuelGuard.Infrastructure.Persistence;

public sealed class EfRepository<TEntity>(FuelGuardDbContext db) : IRepository<TEntity, Guid>
    where TEntity : class
{
    private readonly DbSet<TEntity> _set = db.Set<TEntity>();

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _set.Add(entity);
        return Task.CompletedTask;
    }

    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _set.FindAsync([id], cancellationToken).AsTask();

    public async Task<IReadOnlyList<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _set.AsNoTracking();
        if (predicate is not null)
            query = query.Where(predicate);

        return await query.ToListAsync(cancellationToken);
    }
}
