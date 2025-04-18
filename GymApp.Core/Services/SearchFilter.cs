
public class SearchFilter : SearchFilterBase, IOffsetTermFilter, ITermFilter
{
    public int PgSize { get; set; }

    public string? SearchTerm { get; set; }

    public int PgNumber { get; set; }

    public TermFilter ToTermFilter()
    {
        return new TermFilter
        {
            PgNumber = PgNumber,
            PgSize = PgSize,
            SearchTerm = SearchTerm
        };
    }
}