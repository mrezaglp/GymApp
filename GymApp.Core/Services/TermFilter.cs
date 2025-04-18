
public class TermFilter : IOffsetTermFilter, ITermFilter
{
    public int PgNumber { get; set; }

    public int PgSize { get; set; }

    public string? SearchTerm { get; set; }
}