
public sealed class MsSqlDb : BaseDbInstance
{
    public override IDbContextProvider Provider => IDbContextProvider.msSql;

    public override string ProviderName => "msSql";
}