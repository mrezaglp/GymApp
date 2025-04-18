using System;
using GymApp.Core.Common;
using GymApp.Core.Interfaces;

namespace GymApp.Core.Models
{
    public class Membership : BaseEntity<string>, IAggregateRoot
    {
        public string GymName { get; private set; }
        public string GymId { get; private set; }
        public Gym Gym { get; private set; }
        public string UserId { get; private set; }
        public User User { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        private Membership() { }

        public Membership(string id, string gymName, string gymId, string userId, DateTime startDate, DateTime endDate)
            : base(id)
        {
            GymName = gymName ?? throw new ArgumentNullException(nameof(gymName));
            GymId = gymId ?? throw new ArgumentNullException(nameof(gymId));
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            StartDate = startDate;
            EndDate = endDate;
            AddDomainEvent(new MembershipCreatedEvent(id, userId));
        }

        public bool IsActive(DateTime currentDate)
        {
            return currentDate >= StartDate && currentDate <= EndDate;
        }

        public void Delete()
        {
            IsDeleted = true;
            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new MembershipDeletedEvent(Id));
        }

        public IEntity GetRootEntity()
        {
            return this;
        }
    }

    public record MembershipCreatedEvent(string MembershipId, string UserId) : IDomainEvent;
    public record MembershipDeletedEvent(string MembershipId) : IDomainEvent;
}