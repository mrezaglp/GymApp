using Ardalis.Result;
using FastEndpoints;
using GymApp.Core.Interfaces;
using GymApp.Core.Models;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GymApp.Web.Endpoints;

public class GetAllGymUsersEndpoint(IUnitOfWork _unitOfWork) : Endpoint<GetAllGymUsersRequest, string>
{
    public IUnitOfWork UnitOfWork { get; } = _unitOfWork;

    public override void Configure()
    {
        Get("/gym/users/{gymId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllGymUsersRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.UserId) || string.IsNullOrWhiteSpace(req.GymId))
        {
            await SendAsync(Result<string>.Error("Invalid request data"), cancellation: ct);
            return;
        }

        var users = await _unitOfWork.Repository<Gym>().ListAsync(new GetAllGymUsersSpec(req.GymId));

        await SendAsync(Result<string>.Success($"Currently '{users}' status!"), cancellation: ct);
    }
}
public class GetAllGymUsersRequest
{
    public string UserId { get; set; }
    public string GymId { get; set; }
}


