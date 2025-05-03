using Ardalis.Specification;
using GymApp.Core.Enums;
using GymApp.Core.Models;

public class ActiveSessionsSpec : Specification<GymSession>
{
    public ActiveSessionsSpec(string gymId)
    {
        Query.Where(s => s.GymId == gymId && s.SessionStatus == SessionStatusEnum.Active && !s.IsDeleted);
    }
}