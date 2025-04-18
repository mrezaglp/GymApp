using Ardalis.Specification;
using GymApp.Core.Models;

public class GetAllGymActiveSessionsSpec : Specification<Gym,List<GymSession>>
{
    public GetAllGymActiveSessionsSpec(string id)
    {
        Query.Where(g => g.Id == id);
    }
}
