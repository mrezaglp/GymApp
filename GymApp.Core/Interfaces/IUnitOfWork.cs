using System;
using System.Threading.Tasks;
using GymApp.Core.Common;

namespace GymApp.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IReadRepository<T, TId> ReadRepository<T, TId>() where T : class, IEntity<TId>;
        IWriteRepository<T, TId> WriteRepository<T, TId>() where T : class, IEntity<TId>;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}