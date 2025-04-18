using GymApp.Core.Interfaces;
using GymApp.Core.Enums;
using GymApp.Core.Interfaces;

namespace GymApp.Core.Models;
public class User : BaseEntity<string>,IAggregateRoot
{
    public string Name { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public GenderEnum? Gender { get; set; }
    public UserStatusEnum UserStatus { get; set; } 
    public GymSession? ActiveSession { get; set; }
    public List<GymSession>? Sessions { get; set; }
    public List<Gym> Gyms { get; set; }
    public List<Membership>? Memberships { get; set; }
    public Locker? Locker { get; set; }

    public IEntity GetRootEntity()
    {
        throw new NotImplementedException();
    }
}