
using Ardalis.Specification;
using GymApp.Core.Interfaces;

public interface IDbContext : IDbContextGetter, IDisposable
{
    IEnumerable<TService> GetServices<TService>() where TService : class;

    IEnumerable<object> GetServices(Type tservice);

    TService GetService<TService>() where TService : class;

    TService GetService<TService>(Func<TService, bool> predict) where TService : class;

    TService GetService<TService>(Func<bool> predict) where TService : class;

    void CleanEntityState();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken), bool dispatchIntents = true);

    Task BulkAsync<T>(BulkOperation<T> bulkOperation, CancellationToken cancellationToken = default(CancellationToken), bool dispatchIntents = true) where T : class, IEntity;

    int SaveChanges(CancellationToken cancellationToken = default(CancellationToken), bool dispatchIntents = true);
}