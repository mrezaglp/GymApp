using Ardalis.Result;
using FastEndpoints;
using GymApp.Core.Interfaces;
using GymApp.Core.Models;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GymApp.Web.Endpoints;

public class GetAllGymsEndpoint(IUnitOfWork _unitOfWork) : Endpoint<GetAllGymsRequest, string>
{
    public IUnitOfWork UnitOfWork { get; } = _unitOfWork;

    public override void Configure()
    {
        Get("/gym/status/{userId}/{gymId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllGymsRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.UserId) || string.IsNullOrWhiteSpace(req.GymId))
        {
            await SendAsync(Result<string>.Error("Invalid request data"), cancellation: ct);
            return;
        }

        var gyms = await _unitOfWork.Repository<Gym>().ListAsync();
    
        await SendAsync(Result<string>.Success($"Currently '{gyms}' status!"), cancellation: ct);
    }
}
public class GetAllGymsRequest
{
    public string UserId { get; set; }
    public string GymId { get; set; }
}


