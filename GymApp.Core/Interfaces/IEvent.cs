
namespace MediatR;

public interface IEvent : INotification, Intent
{
    string IntentId { get; }
}