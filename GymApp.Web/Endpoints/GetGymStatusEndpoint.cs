using Ardalis.Result;
using FastEndpoints;
using GymApp.Core.Interfaces;
using GymApp.Core.Models;

namespace GymApp.Web.Endpoints;

public class GetGymStatusEndpoint(IUnitOfWork _unitOfWork) : Endpoint<GetGymStatusRequest, string>
{
    public IUnitOfWork UnitOfWork { get; } = _unitOfWork;

    public override void Configure()
    {
        Get("/gym/status/{userId}/{gymId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetGymStatusRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.UserId) || string.IsNullOrWhiteSpace(req.GymId))
        {
            await SendAsync(Result<string>.Error("Invalid request data"), cancellation: ct);
            return;
        }

        var gym = await _unitOfWork.Repository<Gym>().GetByIdAsync(req.GymId);
        
        var status = gym.ActiveSessions.Count + gym.LockerCount;
    
        await SendAsync(Result<string>.Success($"Currently '{status}' status!"), cancellation: ct);
    }
}
public class GetGymStatusRequest
{
    public string UserId { get; set; }
    public string GymId { get; set; }
}


