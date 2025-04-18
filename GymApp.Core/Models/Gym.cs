using System.Numerics;
using GymApp.Core.Interfaces;
using GymApp.Core.Enums;
using GymApp.Core.Common;

namespace GymApp.Core.Models;

public class Gym :IEntity<string>, IAuditableEntity
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public GenderEnum GymGender { get; set; }
    public TimeOnly OpeningTime { get; set; }
    public TimeOnly ClosingTime { get; set; }
    public int LockerCount { get; set; }
    public List<Locker> Lockers { get; set; } 
    public List<GymSession> ActiveSessions { get; set; }
    public List<User> Members { get; set; }
    public List<Membership> Memberships { get; set; }
    public string Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime CreatedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime? ModifiedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsDeleted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Audit(IReadOnlyDictionary<string, object?> Original, IReadOnlyDictionary<string, object?> Current)
    {
        throw new NotImplementedException();
    }

    public IEntity GetRootEntity()
    {
        throw new NotImplementedException();
    }
}