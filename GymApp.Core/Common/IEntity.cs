using System;
using System.Collections.Generic;

namespace GymApp.Core.Common
{
    public interface IEntity
    {
        object Id { get; }
        DateTime CreatedAt { get; }
        DateTime? ModifiedAt { get; }
        bool IsDeleted { get; }
        IReadOnlyCollection<object> DomainEvents { get; }
        void ClearDomainEvents();
    }

    public interface IEntity<TId> : IEntity
    {
        new TId Id { get; set; }
        new DateTime CreatedAt { get; set; }
        new DateTime? ModifiedAt { get; set; }
        new bool IsDeleted { get; set; }
    }
}