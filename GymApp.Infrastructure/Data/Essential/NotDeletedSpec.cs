
using Ardalis.Specification;
using GymApp.Core.Common;

namespace GymApp.Infrastructure.Data;

public class NotDeletedSpec<T> : Specification<T> where T : class, Core.Common.IEntity<object>
{
    public NotDeletedSpec()
    {
        Query.Where(x => !x.IsDeleted);
    }
}
