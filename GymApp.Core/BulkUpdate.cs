
public record BulkUpdate<T, TProperty>(Func<T, TProperty> Getter, Func<T, TProperty> Setter);