using GymApp.Core.Interfaces;
using GymApp.Core.Interfaces;

namespace GymApp.Core.Models;

public class Membership : BaseEntity<string>,IAggregateRoot
{
    public string GymName { get; set; }
    public Gym Gym { get; set; }
    public User User { get; set; }
    public string UserId { get; set; }
    public string GymId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public IEntity GetRootEntity()
    {
        throw new NotImplementedException();
    }
}