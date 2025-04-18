namespace GymApp.Core.Events;
public static class EntityUpdatedEvent
{
    public static EntityUpdatedEvent<TEntity> WithEntity<TEntity>(TEntity entity) where TEntity : IEntity
    {
        return new EntityUpdatedEvent<TEntity>(entity);
    }
}