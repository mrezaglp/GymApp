using System.Threading.Tasks;
using Ardalis.Specification;
using GymApp.Core.Common;

namespace GymApp.Core.Interfaces
{
    public interface IReadRepository<T, TId> where T : class, Common.IEntity<TId>
    {
        Task<T?> GetByIdAsync(TId id);
        Task<T?> FirstOrDefaultAsync(ISpecification<T> specification);
        Task<List<T>> ListAsync();
        Task<List<T>> ListAsync(ISpecification<T> specification);
        Task<int> CountAsync(ISpecification<T> specification);
    }
}