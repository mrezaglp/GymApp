using GymApp.Core.Models;

public abstract class BaseEntity<PrimaryKey> : BaseDomainEntity
{
    public PrimaryKey Id { get; protected set; }

    protected BaseEntity()
    {
    }

    protected BaseEntity(DateTime createdAt)
        : base(createdAt)
    {
    }

    public override object? GetPrimaryKey()
    {
        return Id;
    }
}