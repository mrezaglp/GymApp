
using GymApp.Core.Interfaces;

public interface IReadRepositoryFactory
{
    IDbContextProvider Provider { get; }

    string DbProviderName { get; }

    IReadRepository<TEntity>? Create<TEntity>(IDbContext context) where TEntity : class, IMuteEntity;
}