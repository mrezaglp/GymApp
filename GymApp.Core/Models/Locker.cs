using GymApp.Core.Interfaces;
using GymApp.Core.Interfaces;
using GymApp.Core.Models;
using MediatR;

namespace GymApp.Core.Models;
public class Locker : BaseEntity<string>,IAggregateRoot
{
    public int Number { get; set; }
    public bool IsOccupied { get; set; }

    public string? OccupiedByUserId { get; set; }
    public User? OccupiedByUser { get; set; }

    public string GymId { get; set; }
    public Gym Gym { get; set; }

    public IEntity GetRootEntity()
    {
        throw new NotImplementedException();
    }
}
