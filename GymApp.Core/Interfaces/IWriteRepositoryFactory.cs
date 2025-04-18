
using GymApp.Core.Interfaces;

public interface IWriteRepositoryFactory
{
    IDbContextProvider Provider { get; }

    string DbProviderName { get; }

    IRepository<TEntity>? Create<TEntity>(IDbContext context) where TEntity : class, IAggregateRoot;
}