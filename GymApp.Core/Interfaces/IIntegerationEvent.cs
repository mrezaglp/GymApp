
using MediatR;
namespace GymApp.Core.Interfaces;
public interface IIntegerationEvent : IEvent, INotification, Intent
{
}