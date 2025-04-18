
using GymApp.Core.Interfaces;
using MediatR;

public interface IEventMapper
{
    IIntegerationEvent ToIntergerationaEvent(IEvent @event);

    bool IsSatisfiedWith(IEvent @event);

    bool IsSatisfiedWithAny(IEnumerable<IEvent> events);
}