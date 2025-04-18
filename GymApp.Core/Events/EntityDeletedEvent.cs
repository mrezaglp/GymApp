using GymApp.Core.Interfaces;

namespace GymApp.Core.Events;
public static class EntityDeletedEvent
{
    public static EntityDeletedEvent<TEntity> WithEntity<TEntity>(TEntity entity) where TEntity : IEntity
    {
        return new EntityDeletedEvent<TEntity>(entity);
    }
}