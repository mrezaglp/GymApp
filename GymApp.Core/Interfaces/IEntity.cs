
using System.ComponentModel.DataAnnotations.Schema;
using MediatR;
namespace GymApp.Core.Interfaces;
public interface IEntity : IMuteEntity, ICreatedDateTime, ISoftDelete, IVisibility, IModifiedDateTime, IVersioned
{
    [NotMapped]
    IEnumerable<BaseDomainEvent> DomainEvents { get; }

    [NotMapped]
    IEnumerable<BaseIntegerationEvent> IntegerationlEvents { get; }

    [NotMapped]
    IEnumerable<ICommand<IEvent>> DomainCommands { get; }

    object? GetPrimaryKey();
}