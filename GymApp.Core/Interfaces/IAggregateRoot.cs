using GymApp.Core.Interfaces;

namespace GymApp.Core.Interfaces;
public interface IAggregateRoot : IEntity, IMuteEntity, ICreatedDateTime, ISoftDelete, IVisibility, IModifiedDateTime, IVersioned
{
    IEntity GetRootEntity();
}