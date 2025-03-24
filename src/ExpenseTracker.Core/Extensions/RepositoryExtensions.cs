using System.Linq.Expressions;
using ExpenseTracker.Core.Common;
using ExpenseTracker.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Core.Extensions;

public static class RepositoryExtensions
{
    public static IQueryable<T> GetAll<T>(this IRepository<T> repository) where T : BaseEntity
    {
        return repository.Query();
    }

    public static async Task<T?> FirstOrDefaultAsync<T>(
        this IRepository<T> repository,
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default) where T : BaseEntity
    {
        return await repository.Query().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public static async Task<T?> FirstOrDefaultAsync<T>(
        this IQueryable<T> query,
        CancellationToken cancellationToken = default) where T : BaseEntity
    {
        return await query.FirstOrDefaultAsync(cancellationToken);
    }
}
