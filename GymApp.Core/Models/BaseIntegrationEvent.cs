using GymApp.Core.Interfaces;
using Gym.ValueObjects;
using MediatR;

public abstract class BaseIntegerationEvent : IIntegerationEvent, IEvent, INotification, Intent
{
    public DateTime DateOccurred { get; private set; }

    public string IntentId { get; private set; }

    protected BaseIntegerationEvent()
    {
        IntentId = IDentifiable.New;
        DateOccurred = DateTimeService.Current.Now;
    }

    protected BaseIntegerationEvent(string intentId)
    {
        IntentId = intentId ?? IDentifiable.New;
        DateOccurred = DateTimeService.Current.Now;
    }

    public virtual string GetKey()
    {
        return IntentId;
    }
}