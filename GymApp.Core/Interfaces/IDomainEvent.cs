
using MediatR;
namespace GymApp.Core.Interfaces;
public interface IDomainEvent : IEvent, INotification, Intent
{
}