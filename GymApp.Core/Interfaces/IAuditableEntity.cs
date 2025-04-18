
public interface IAuditableEntity
{
    void Audit(IReadOnlyDictionary<string, object?> Original, IReadOnlyDictionary<string, object?> Current);
}