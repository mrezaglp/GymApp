using System.Threading.Tasks;
using GymApp.Core.Common;
using GymApp.Core.Interfaces;
using GymApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GymApp.Infrastructure.Repositories
{
    public class EfWriteRepository<T, TId> : IWriteRepository<T, TId> where T : class, IEntity<TId>
    {
        private readonly AppDbContext _dbContext;

        public EfWriteRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }

        public async Task DeleteAsync(TId id)
        {
            var entity = await _dbContext.Set<T>().FirstOrDefaultAsync(e => Equals(e.Id, id));
            if (entity != null)
            {
                entity.IsDeleted = true;
                _dbContext.Set<T>().Update(entity);
            }
        }
    }
}