
using GymApp.Core.Interfaces;

public interface ICommandDbContext<out TDbInstance> : IBaseDbContextServiceCollection<TDbInstance>, IDbContextInfrastructure<IServiceProvider>, IDbContext<TDbInstance>, IDbContext, IDbContextGetter, IDisposable where TDbInstance : BaseDbInstance
{
    T AddCommand<T>(T entery, DbContextChangeTracker.DbContextEntryState state, Action action) where T : class, IMuteEntity;

    IEnumerable<T> AddCommand<T>(IEnumerable<T> enteries, DbContextChangeTracker.DbContextEntryState state, Action action) where T : class, IMuteEntity;
}