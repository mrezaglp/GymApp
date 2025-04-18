using Ardalis.Result;
using FastEndpoints;
using GymApp.Core.Interfaces;
using GymApp.Core.Models;

namespace GymApp.Web.Endpoints;

public class GetAllGymActiveSessionsEndpoint(IUnitOfWork _unitOfWork) : Endpoint<GetAllGymActiveSessionsRequest, string>
{
    public IUnitOfWork UnitOfWork { get; } = _unitOfWork;

    public override void Configure()
    {
        Get("/gym/sessions/{gymId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllGymActiveSessionsRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.UserId) || string.IsNullOrWhiteSpace(req.GymId))
        {
            await SendAsync(Result<string>.Error("Invalid request data"), cancellation: ct);
            return;
        }

        var activeSessions = await _unitOfWork.Repository<Gym>().ListAsync(new GetAllGymActiveSessionsSpec(req.GymId));
       
        await SendAsync(Result<string>.Success($"Currently '{activeSessions}' status!"), cancellation: ct);
    }
}
public class GetAllGymActiveSessionsRequest
{
    public string UserId { get; set; }
    public string GymId { get; set; }
}


