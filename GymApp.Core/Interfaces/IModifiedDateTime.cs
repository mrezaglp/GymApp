namespace GymApp.Core.Interfaces;
public interface IModifiedDateTime
{
    DateTime? ModifiedAt { get; }

    void Modified(bool registerEntityUpdatedEvent = false);
}