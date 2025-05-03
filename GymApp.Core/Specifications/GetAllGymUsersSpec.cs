using Ardalis.Specification;
using GymApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GymApp.Application.Specifications
{
    public class GetAllGymUsersSpec : Specification<Gym>
    {
        public GetAllGymUsersSpec(string gymId)
        {
            Query.Where(g => g.Id == gymId && !g.IsDeleted)
                 .Include(g => g.Memberships.Where(m => !m.IsDeleted && m.StartDate <= DateTime.UtcNow && m.EndDate >= DateTime.UtcNow));
        }
    }
}