
using GymApp.Core.Interfaces;

public interface ICommandDbContextAsync<out TDbInstance> : ICommandDbContext<TDbInstance>, IBaseDbContextServiceCollection<TDbInstance>, IDbContextInfrastructure<IServiceProvider>, IDbContext<TDbInstance>, IDbContext, IDbContextGetter, IDisposable where TDbInstance : BaseDbInstance
{
    Task<T> AddCommand<T>(T entery, DbContextChangeTracker.DbContextEntryState state, Func<Task<long>> func) where T : class, IMuteEntity;

    Task<IEnumerable<T>> AddCommand<T>(IEnumerable<T> enteries, DbContextChangeTracker.DbContextEntryState state, Func<Task<long>> func) where T : class, IMuteEntity;
}