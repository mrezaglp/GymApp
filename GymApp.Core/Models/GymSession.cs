using System;
using GymApp.Core.Common;
using GymApp.Core.Enums;
using GymApp.Core.Interfaces;

namespace GymApp.Core.Models
{
    public class GymSession : BaseEntity<string>, IAggregateRoot
    {
        public string Name { get; private set; }
        public string GymId { get; private set; }
        public Gym Gym { get; private set; }
        public string UserId { get; private set; }
        public User User { get; private set; }
        public string LockerId { get; private set; }
        public Locker Locker { get; private set; }
        public SessionStatusEnum SessionStatus { get; private set; }
        public DateTime CheckInTime { get; private set; }
        public DateTime? CheckOutTime { get; private set; }

        private GymSession() { }

        public GymSession(string id, string name, string gymId, string userId, DateTime checkInTime)
            : base(id)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            GymId = gymId ?? throw new ArgumentNullException(nameof(gymId));
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            CheckInTime = checkInTime;
            SessionStatus = SessionStatusEnum.Active;
            AddDomainEvent(new GymSessionCreatedEvent(id, userId, gymId));
        }

        public void AssignLocker(string lockerId)
        {
            LockerId = lockerId ?? throw new ArgumentNullException(nameof(lockerId));
            AddDomainEvent(new LockerAssignedEvent(lockerId, Id));
        }

        public void CheckOut(DateTime checkOutTime)
        {
            CheckOutTime = checkOutTime;
            SessionStatus = SessionStatusEnum.NotActive;
            AddDomainEvent(new GymSessionEndedEvent(Id, UserId, GymId));
        }

        public void Delete()
        {
            IsDeleted = true;
            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new GymSessionDeletedEvent(Id));
        }

        public IEntity GetRootEntity()
        {
            return this;
        }
    }

    public record GymSessionCreatedEvent(string SessionId, string UserId, string GymId) : IDomainEvent;
    public record LockerAssignedEvent(string LockerId, string SessionId) : IDomainEvent;
    public record GymSessionEndedEvent(string SessionId, string UserId, string GymId) : IDomainEvent;
    public record GymSessionDeletedEvent(string SessionId) : IDomainEvent;
}