
public abstract class BaseDbInstance
{
    public abstract IDbContextProvider Provider { get; }

    public abstract string ProviderName { get; }
}