namespace GymApp.Core.Interfaces;
public interface IReadRepository<T> : IBaseReadRepository<T> where T : class, IMuteEntity
{
}