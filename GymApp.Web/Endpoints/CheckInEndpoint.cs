using Ardalis.Result;
using FastEndpoints;
using GymApp.Application.Specifications;
using GymApp.Core.Enums;
using GymApp.Core.Interfaces;
using GymApp.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace GymApp.Web.Endpoints
{
    public class CheckInEndpoint : Endpoint<CheckInEndpointRequest, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckInEndpoint(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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

            // Validate membership
            var membershipSpec = new ActiveMembershipSpec(req.UserId, gymId, DateTime.UtcNow);
            var membership = await _unitOfWork.ReadRepository<Membership, string>().FirstOrDefaultAsync(membershipSpec);
            if (membership == null)
            {
                await SendAsync(Result<string>.Error("No active membership found"), cancellation: ct);
                return;
            }

            // Check if user is already checked in
            if (user.UserStatus == UserStatusEnum.Active)
            {
                await SendAsync(Result<string>.Error("User is already checked in"), cancellation: ct);
                return;
            }

            // Create session
            var sessionId = Guid.NewGuid().ToString();
            var checkInTime = DateTime.UtcNow;
            var sessionName = $"Session_{user.Name}_{checkInTime:yyyyMMddHHmmss}";
            var session = new GymSession(sessionId, sessionName, gymId, req.UserId, checkInTime);

            // Assign locker
            var lockerSpec = new AvailableLockerSpec(gymId);
            var locker = await _unitOfWork.ReadRepository<Locker, string>().FirstOrDefaultAsync(lockerSpec);
            if (locker == null)
            {
                await SendAsync(Result<string>.Error("No available lockers"), cancellation: ct);
                return;
            }

            locker.Occupy(req.UserId);
            session.AssignLocker(locker.Id);

            // Update user status
            user.CheckIn(session);

            // Update gym occupancy
            gym.IncrementOccupancy();

            // Persist changes
            await _unitOfWork.WriteRepository<GymSession, string>().AddAsync(session);
            await _unitOfWork.WriteRepository<Locker, string>().UpdateAsync(locker);
            await _unitOfWork.WriteRepository<User, string>().UpdateAsync(user);
            await _unitOfWork.WriteRepository<Gym, string>().UpdateAsync(gym);

            await _unitOfWork.SaveChangesAsync(ct);

            await SendAsync(Result<string>.Success($"User '{req.UserId}' checked in successfully!"), cancellation: ct);
        }
    }

    public class CheckInEndpointRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string CheckInToken { get; set; } = string.Empty;
    }
}