using Ardalis.Result;
using FastEndpoints;
using GymApp.Core.Interfaces;
using GymApp.Core.Models;

public class GetGymEndpoint : EndpointWithoutRequest<Result<GymDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetGymEndpoint(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override void Configure()
    {
        Get("/gyms/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<string>("id");
        var gym = await _unitOfWork.ReadRepository<Gym, string>().GetByIdAsync(id);
        if (gym == null)
        {
            await SendAsync(Result<GymDto>.Error("Gym not found"), cancellation: ct);
            return;
        }

        var dto = new GymDto
        {
            Id = gym.Id,
            Name = gym.Name,
            OccupancyCount = gym.OccupancyCount
        };
        await SendAsync(Result<GymDto>.Success(dto), cancellation: ct);
    }
}

public class GymDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int OccupancyCount { get; set; }
}