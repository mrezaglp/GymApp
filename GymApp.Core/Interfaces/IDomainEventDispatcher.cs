
using GymApp.Core.Models;
using MediatR;

public interface IDomainEventDispatcher
{
    IAsyncEnumerable<IEvent?> ExecuteCommands(IEnumerable<BaseDomainEntity> entitiesWithCommands);

    Task DispatchAndClearEvents(IEnumerable<IEvent> onlyEvents);

    Task DispatchAndClearEvents(IEnumerable<BaseDomainEntity> entitiesWithEvents);
}