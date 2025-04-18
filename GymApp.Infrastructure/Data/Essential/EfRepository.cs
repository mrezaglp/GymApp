
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using GymApp.Core.Common;
using Microsoft.EntityFrameworkCore;

namespace GymApp.Infrastructure.Data;

public class EfRepository<T, TId> : IRepository<T, TId> 
    where T : class, Core.Common.IEntity<TId>, IAuditableEntity
{
    private readonly DbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public EfRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    public async Task<T> AddAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        entity.ModifiedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        entity.IsDeleted = true;
        entity.ModifiedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<T?> GetByIdAsync(TId id)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id!.Equals(id) && !x.IsDeleted);
    }

    public async Task<IReadOnlyList<T>> ListAsync()
    {
        return await _dbSet.Where(x => !x.IsDeleted).ToListAsync();
    }

    public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).ToListAsync();
    }

    public async Task<int> CountAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).CountAsync();
    }

    public async Task<bool> ExistsAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).AnyAsync();
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        var evaluator = new SpecificationEvaluator();
        return evaluator.GetQuery(_dbSet.AsQueryable().Where(x => !x.IsDeleted), spec);
    }
}
