
public static class SpecificationExtensions
{
    public static BulkOperation<T> GetBulkOperation<T>(this IBulkSpecification<T> spec, IQueryable<T> q)
    {
        return new BulkOperation<T>(spec.BulkUpdates, spec.BulkDelete, q);
    }
}