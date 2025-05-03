using GymApp.Core.Common;

namespace GymApp.Core.Interfaces
{
    public interface IAggregateRoot
    {
        IEntity GetRootEntity();
    }
}