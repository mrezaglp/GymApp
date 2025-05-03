using Ardalis.Specification;
using GymApp.Core.Enums;
using GymApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GymApp.Application.Specifications
{
    public class GetAllGymActiveSessionsSpec : Specification<Gym>
    {
        public GetAllGymActiveSessionsSpec(string gymId)
        {
            Query.Where(g => g.Id == gymId && !g.IsDeleted)
                 .Include(g => g.ActiveSessions.Where(s => s.SessionStatus == SessionStatusEnum.Active && !s.IsDeleted));
        }
    }
}