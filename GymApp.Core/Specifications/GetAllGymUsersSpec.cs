using Ardalis.Specification;
using GymApp.Core.Models;

public class GetAllGymUsersSpec : Specification<Gym,List<User>>
{
    public GetAllGymUsersSpec(string id)
    {
        Query.Where(g => g.Id == id);
    }
}
