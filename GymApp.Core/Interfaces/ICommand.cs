
namespace MediatR;

public interface ICommand<out TEvent> : IRequest<TEvent>, IBaseRequest, Intent where TEvent : IEvent
{
    TEvent? CarriedEvent { get; }

    string Id { get; }
}