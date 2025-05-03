using System;
using System.Collections.Generic;
using GymApp.Core.Common;
using GymApp.Core.Enums;
using GymApp.Core.Interfaces;

namespace GymApp.Core.Models
{
    public class Gym : BaseEntity<string>, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Address { get; private set; }
        public string PhoneNumber { get; private set; }
        public GenderEnum GymGender { get; private set; }
        public TimeOnly OpeningTime { get; private set; }
        public TimeOnly ClosingTime { get; private set; }
        public int LockerCount { get; private set; }
        public int OccupancyCount { get; private set; }
        public List<Locker> Lockers { get; private set; } = new();
        public List<GymSession> ActiveSessions { get; private set; } = new();
        public List<User> Members { get; private set; } = new();
        public List<Membership> Memberships { get; private set; } = new();

        private Gym() { }

        public Gym(string id, string name, string address, string phoneNumber, GenderEnum gymGender, TimeOnly openingTime, TimeOnly closingTime, int lockerCount)
            : base(id)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            GymGender = gymGender;
            OpeningTime = openingTime;
            ClosingTime = closingTime;
            LockerCount = lockerCount >= 0 ? lockerCount : throw new ArgumentException("LockerCount cannot be negative.", nameof(lockerCount));
            OccupancyCount = 0;
            AddDomainEvent(new GymCreatedEvent(id, name));
        }

        public void IncrementOccupancy()
        {
            OccupancyCount++;
            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new GymOccupancyUpdatedEvent(Id, OccupancyCount));
        }

        public void DecrementOccupancy()
        {
            if (OccupancyCount > 0)
            {
                OccupancyCount--;
                ModifiedAt = DateTime.UtcNow;
                AddDomainEvent(new GymOccupancyUpdatedEvent(Id, OccupancyCount));
            }
        }

        public void AddLocker(Locker locker)
        {
            Lockers.Add(locker);
            AddDomainEvent(new LockerAddedEvent(locker.Id, Id));
        }

        public void Delete()
        {
            IsDeleted = true;
            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new GymDeletedEvent(Id));
        }

        public IEntity GetRootEntity()
        {
            return this;
        }
    }

    public record GymCreatedEvent(string GymId, string Name) : IDomainEvent;
    public record LockerAddedEvent(string LockerId, string GymId) : IDomainEvent;
    public record GymOccupancyUpdatedEvent(string GymId, int OccupancyCount) : IDomainEvent;
    public record GymDeletedEvent(string GymId) : IDomainEvent;
}