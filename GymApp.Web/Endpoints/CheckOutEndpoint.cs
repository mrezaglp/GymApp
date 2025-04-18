using Ardalis.Result;
using FastEndpoints;
using GymApp.Core.Interfaces;
using GymApp.Core.Models;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GymApp.Web.Endpoints;

public class CheckOutEndpoint(IUnitOfWork _unitOfWork) : Endpoint<CheckOutEndpointRequest, string>
{
    public IUnitOfWork UnitOfWork { get; } = _unitOfWork;

    public override void Configure()
    {
        Post("/gym/checkOut");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CheckOutEndpointRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.UserId) || string.IsNullOrWhiteSpace(req.CheckOutToken))
        {
            await SendAsync(Result<string>.Error("Invalid request data"), cancellation: ct);
            return;
        }


        // de occupy locker
        // update gym occupancy
        // update user status

        var repo = _unitOfWork.Repository<GymSession>();
        var session = await repo.GetByIdAsync(req.UserId);
         
        session.CheckOutTime = DateTime.Now;
        session.SessionStatus = Core.Enums.SessionStatusEnum.NotActive;

        await repo.UpdateAsync(session);
        await _unitOfWork.SaveChangesAsync();

        await SendAsync(Result<string>.Success($"User '{req.UserId}' checked PUT successfully!"), cancellation: ct);
    }
}
public class CheckOutEndpointRequest
{
    public string UserId { get; set; }
    public string CheckOutToken { get; set; }
}


