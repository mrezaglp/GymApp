
public sealed class BulkOperation<T>
{
    public IEnumerable<BulkUpdate<T, object>> Bulks { get; private set; }

    public bool BulkDelete { get; private set; }

    public IQueryable<T> Query { get; private set; }

    private BulkOperation()
    {
    }

    internal BulkOperation(IEnumerable<BulkUpdate<T, object>> bulks, bool bulkDelete, IQueryable<T> query)
    {
        Bulks = bulks;
        BulkDelete = bulkDelete;
        Query = query;
    }
}