using GymApp.Core.Common;
using GymApp.Core.Enums;
using GymApp.Core.Interfaces;
using GymApp.Core.Interfaces;

namespace GymApp.Core.Models;
public class GymSession : IEntity<string>, IAuditableEntity
  {  public string Name { get; set; }
    public Gym Gym { get; set; }
    public User User { get; set; }
    public string UserId { get; set; }
    public string LockerId { get; set; }
    public Locker Locker { get; set; }
    public SessionStatusEnum SessionStatus { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
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