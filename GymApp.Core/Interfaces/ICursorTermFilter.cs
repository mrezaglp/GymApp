
public interface ICursorTermFilter : ITermFilter
{
    long? Cursor { get; set; }

    CursorDirectionEnum? Direction { get; set; }
}