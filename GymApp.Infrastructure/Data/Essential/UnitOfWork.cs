using GymApp.Core.Common;
using GymApp.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymApp.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;
    private readonly Dictionary<Type, object> _repositories = new();
    private readonly Dictionary<Type, object> _readRepositories = new();

    public UnitOfWork(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IRepository<T, string> Repository<T>() where T : class, IEntity<string>, IAuditableEntity
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            var repoInstance = new EfRepository<T, string>(_dbContext);
            _repositories[type] = repoInstance;
        }

        return (IRepository<T, string>)_repositories[type]!;
    }

    public IReadRepository<T, string> ReadRepository<T>() where T : class, IEntity<string>, IAuditableEntity
    {
        var type = typeof(T);
        if (!_readRepositories.ContainsKey(type))
        {
            var readRepoInstance = new EfReadRepository<T, string>(_dbContext);
            _readRepositories[type] = readRepoInstance;
        }

        return (IReadRepository<T, string>)_readRepositories[type]!;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}
