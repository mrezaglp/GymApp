using Ardalis.Specification;
using GymApp.Core.Models;

namespace GymApp.Application.Specifications
{
    public class AvailableLockerSpec : Specification<Locker>
    {
        public AvailableLockerSpec(string gymId)
        {
            Query.Where(l => l.GymId == gymId && !l.IsOccupied && !l.IsDeleted);
        }
    }
}