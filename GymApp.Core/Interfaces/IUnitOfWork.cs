// IUnitOfWork.cs
using GymApp.Core.Common;

namespace GymApp.Core.Interfaces;

public interface IUnitOfWork
{
    IRepository<T, string> Repository<T>() where T : class, IEntity<string>, IAuditableEntity;
    IReadRepository<T, string> ReadRepository<T>() where T : class, IEntity<string>, IAuditableEntity;
    Task<int> SaveChangesAsync();
}
