using System;
using GymApp.Core.Common;
using GymApp.Core.Interfaces;

namespace GymApp.Core.Models
{
    public class Locker : BaseEntity<string>, IAggregateRoot
    {
        public int Number { get; private set; }
        public bool IsOccupied { get; private set; }
        public string? OccupiedByUserId { get; private set; }
        public User? OccupiedByUser { get; private set; }
        public string GymId { get; private set; }
        public Gym Gym { get; private set; }

        private Locker() { }

        public Locker(string id, int number, string gymId)
            : base(id)
        {
            Number = number >= 0 ? number : throw new ArgumentException("Number cannot be negative.", nameof(number));
            GymId = gymId ?? throw new ArgumentNullException(nameof(gymId));
            IsOccupied = false;
            AddDomainEvent(new LockerCreatedEvent(id, number, gymId));
        }

        public void Occupy(string userId)
        {
            IsOccupied = true;
            OccupiedByUserId = userId ?? throw new ArgumentNullException(nameof(userId));
            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new LockerOccupiedEvent(Id, userId));
        }

        public void Release()
        {
            IsOccupied = false;
            OccupiedByUserId = null;
            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new LockerReleasedEvent(Id));
        }

        public void Delete()
        {
            IsDeleted = true;
            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new LockerDeletedEvent(Id));
        }

        public IEntity GetRootEntity()
        {
            return this;
        }
    }

    public record LockerCreatedEvent(string LockerId, int Number, string GymId) : IDomainEvent;
    public record LockerOccupiedEvent(string LockerId, string UserId) : IDomainEvent;
    public record LockerReleasedEvent(string LockerId) : IDomainEvent;
    public record LockerDeletedEvent(string LockerId) : IDomainEvent;
}