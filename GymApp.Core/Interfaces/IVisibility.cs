namespace GymApp.Core.Interfaces;
public interface IVisibility
{
    bool IsVisibled { get; }

    void Visible(bool registerEntityUpdatedEvent = true);

    void InVisible(bool registerEntityUpdatedEvent = true);
}