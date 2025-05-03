using Ardalis.Result;
using FastEndpoints;
using GymApp.Core.Enums;
using GymApp.Core.Interfaces;
using GymApp.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace GymApp.Web.Endpoints
{
    public class CheckOutEndpoint : Endpoint<CheckOutEndpointRequest, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckOutEndpoint(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override void Configure()
        {
            Post("/gym/checkOut");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CheckOutEndpointRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.UserId) || string.IsNullOrWhiteSpace(req.CheckInToken))
            {
                await SendAsync(Result<string>.Error("Invalid request data"), cancellation: ct);
                return;
            }

            // Assume CheckInToken is GymId
            var gymId = req.CheckInToken;

            // Validate user
            var user = await _unitOfWork.ReadRepository<User, string>().GetByIdAsync(req.UserId);
            if (user == null)
            {
                await SendAsync(Result<string>.Error("User not found"), cancellation: ct);
                return;
            }

            // Validate gym
            var gym = await _unitOfWork.ReadRepository<Gym, string>().GetByIdAsync(gymId);
            if (gym == null)
            {
                await SendAsync(Result<string>.Error("Gym not found"), cancellation: ct);
                return;
            }

            // Check if user is checked in
            if (user.UserStatus != UserStatusEnum.Active || user.ActiveSession == null)
            {
                await SendAsync(Result<string>.Error("User is not checked in"), cancellation: ct);
                return;
            }

            // Get active session
            var session = await _unitOfWork.ReadRepository<GymSession, string>().GetByIdAsync(user.ActiveSession.Id);
            if (session == null || session.GymId != gymId || session.SessionStatus != SessionStatusEnum.Active)
            {
                await SendAsync(Result<string>.Error("No active session found for this user in this gym"), cancellation: ct);
                return;
            }

            // Release locker if assigned
            if (!string.IsNullOrEmpty(session.LockerId))
            {
                var locker = await _unitOfWork.ReadRepository<Locker, string>().GetByIdAsync(session.LockerId);
                if (locker != null)
                {
                    locker.Release();
                    await _unitOfWork.WriteRepository<Locker, string>().UpdateAsync(locker);
                }
            }

            // Update session
            session.CheckOut(DateTime.UtcNow);

            // Update user status
            user.CheckOut();

            // Update gym occupancy
            gym.DecrementOccupancy();

            // Persist changes
            await _unitOfWork.WriteRepository<GymSession, string>().UpdateAsync(session);
            await _unitOfWork.WriteRepository<User, string>().UpdateAsync(user);
            await _unitOfWork.WriteRepository<Gym, string>().UpdateAsync(gym);

            await _unitOfWork.SaveChangesAsync(ct);

            await SendAsync(Result<string>.Success($"User '{req.UserId}' checked out successfully!"), cancellation: ct);
        }
    }

    public class CheckOutEndpointRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string CheckInToken { get; set; } = string.Empty; // Assumed to be GymId
    }
}