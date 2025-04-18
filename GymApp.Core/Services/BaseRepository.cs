
using Ardalis.Specification;

public abstract class BaseRepository<T>
{
    protected readonly ISpecificationEvaluator specificationEvaluator;

    public IDbContextProvider Provider => Db.DbContextProvider;

    public IDbContext Db { get; protected set; }

    protected BaseRepository(IDbContext dbContext, ISpecificationEvaluator specificationEvaluator)
    {
        Db = dbContext;
        this.specificationEvaluator = specificationEvaluator;
    }
}