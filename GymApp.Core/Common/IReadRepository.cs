
using Ardalis.Specification;

namespace GymApp.Core.Common;

public interface IReadRepository<T, TId> where T : class, IEntity<TId>
{
    Task<T?> GetByIdAsync(TId id);
    Task<IReadOnlyList<T>> ListAsync();
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    Task<int> CountAsync(ISpecification<T> spec);
    Task<bool> ExistsAsync(ISpecification<T> spec);
}   