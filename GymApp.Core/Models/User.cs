using System;
using System.Collections.Generic;
using GymApp.Core.Common;
using GymApp.Core.Enums;
using GymApp.Core.Interfaces;

namespace GymApp.Core.Models
{
    public class User : BaseEntity<string>, IAggregateRoot
    {
        public string Name { get; private set; }
        public string? Address { get; private set; }
        public string? PhoneNumber { get; private set; }
        public GenderEnum? Gender { get; private set; }
        public UserStatusEnum UserStatus { get; private set; }
        public GymSession? ActiveSession { get; private set; }
        public List<GymSession> Sessions { get; private set; } = new();
        public List<Gym> Gyms { get; private set; } = new();
        public List<Membership> Memberships { get; private set; } = new();
        public Locker? Locker { get; private set; }

        private User() { }

        public User(string id, string name, string? address, string? phoneNumber, GenderEnum? gender)
            : base(id)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Address = address;
            PhoneNumber = phoneNumber;
            Gender = gender;
            UserStatus = UserStatusEnum.NotActive;
            AddDomainEvent(new UserCreatedEvent(id, name));
        }

        public void CheckIn(GymSession session)
        {
            UserStatus = UserStatusEnum.Active;
            ActiveSession = session ?? throw new ArgumentNullException(nameof(session));
            Sessions.Add(session);
            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new UserCheckedInEvent(Id));
        }

        public void CheckOut()
        {
            UserStatus = UserStatusEnum.NotActive;
            ActiveSession = null;
            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new UserCheckedOutEvent(Id));
        }

        public void Delete()
        {
            IsDeleted = true;
            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new UserDeletedEvent(Id));
        }

        public IEntity GetRootEntity()
        {
            return this;
        }
    }

    public record UserCreatedEvent(string UserId, string Name) : IDomainEvent;
    public record UserCheckedInEvent(string UserId) : IDomainEvent;
    public record UserCheckedOutEvent(string UserId) : IDomainEvent;
    public record UserDeletedEvent(string UserId) : IDomainEvent;
}