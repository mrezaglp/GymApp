
public interface IDbContextGetter
{
    IDbContextProvider DbContextProvider { get; }

    string DbProviderName { get; }

    T GetContext<T>() where T : class;
}