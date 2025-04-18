
public interface IDbContext<out TDbInstance> : IDbContext, IDbContextGetter, IDisposable where TDbInstance : BaseDbInstance
{
}