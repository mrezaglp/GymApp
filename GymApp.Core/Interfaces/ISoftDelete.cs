namespace GymApp.Core.Interfaces;
public interface ISoftDelete
{
    bool IsDeleted { get; }

    void Delete(bool registerEntityDeletedEvent = true);
}