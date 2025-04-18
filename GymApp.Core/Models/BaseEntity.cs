using System;
using System.Collections.Generic;

namespace GymApp.Core.Common
{
    public abstract class BaseEntity<TId> : IEntity<TId>
    {
        public TId Id { get; set; } = default!;
        object IEntity.Id => Id!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
        private readonly List<object> _domainEvents = new();
        public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

        protected BaseEntity() { }

        protected BaseEntity(TId id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        protected void AddDomainEvent(object domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}