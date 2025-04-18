
using GymApp.Core.Interfaces;

public interface IRepository<T> : IWriteRepository<T>, IBaseReadRepository<T> where T : class, IAggregateRoot
{
}