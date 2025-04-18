using System.Threading.Tasks;
using GymApp.Core.Common;

namespace GymApp.Core.Interfaces
{
    public interface IWriteRepository<T, TId> where T : class, IEntity<TId>
    {
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(TId id);
    }
}