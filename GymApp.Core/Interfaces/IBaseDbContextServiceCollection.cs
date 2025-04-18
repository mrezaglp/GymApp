
public interface IBaseDbContextServiceCollection<out TDbInstance> : IDbContextInfrastructure<IServiceProvider>, IDbContext<TDbInstance>, IDbContext, IDbContextGetter, IDisposable where TDbInstance : BaseDbInstance
{
    DbContextChangeTracker ChangeTracker { get; }
}