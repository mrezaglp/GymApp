using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymApp.Core.Common;
using GymApp.Core.Interfaces;
using GymApp.Infrastructure.Data;
using GymApp.Infrastructure.Repositories;

namespace GymApp.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly Dictionary<Type, object> _readRepositories = new();
        private readonly Dictionary<Type, object> _writeRepositories = new();

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IReadRepository<T, TId> ReadRepository<T, TId>() where T : class, IEntity<TId>
        {
            var key = typeof(T);
            if (_readRepositories.TryGetValue(key, out var repo))
                return (IReadRepository<T, TId>)repo;

            var newRepo = new EfReadRepository<T, TId>(_dbContext);
            _readRepositories[key] = newRepo;
            return newRepo;
        }

        public IWriteRepository<T, TId> WriteRepository<T, TId>() where T : class, IEntity<TId>
        {
            var key = typeof(T);
            if (_writeRepositories.TryGetValue(key, out var repo))
                return (IWriteRepository<T, TId>)repo;

            var newRepo = new EfWriteRepository<T, TId>(_dbContext);
            _writeRepositories[key] = newRepo;
            return newRepo;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}