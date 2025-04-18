using Ardalis.Result;
using FastEndpoints;
using GymApp.Core.Interfaces;
using GymApp.Core.Models;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GymApp.Web.Endpoints;

public class CheckInEndpoint(IUnitOfWork _unitOfWork) : Endpoint<CheckInEndpointRequest, string>
{
    public IUnitOfWork UnitOfWork { get; } = _unitOfWork;

    public override void Configure()
    {
        Post("/gym/checkIn");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CheckInEndpointRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.UserId) || string.IsNullOrWhiteSpace(req.CheckInToken))
        {
            await SendAsync(Result<string>.Error("Invalid request data"), cancellation: ct);
            return;
        }

        var checkIn = new GymSession
        {
            UserId = req.UserId,
            CheckInTime = DateTime.Now

        };
         // assign and occupy locker
         // update gym occupancy
         // update user status

        var repo = _unitOfWork.Repository<GymSession>();

        await repo.AddAsync(checkIn);
        await _unitOfWork.SaveChangesAsync();

        await SendAsync(Result<string>.Success($"User '{req.UserId}' checked in successfully!"), cancellation: ct);
    }
}
public class CheckInEndpointRequest
{
    public string UserId { get; set; }
    public string CheckInToken { get; set; }
}


