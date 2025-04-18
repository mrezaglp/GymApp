using Ardalis.Specification;
using GymApp.Core.Models;

namespace GymApp.Application.Specifications
{
    public class ActiveMembershipSpec : Specification<Membership>
    {
        public ActiveMembershipSpec(string userId, string gymId, DateTime currentDate)
        {
            Query.Where(m => m.UserId == userId && m.GymId == gymId && !m.IsDeleted && m.StartDate <= currentDate && m.EndDate >= currentDate);
        }
    }
}