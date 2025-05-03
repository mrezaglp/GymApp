using Ardalis.Result;
using FastEndpoints;
using GymApp.Core.Interfaces;
using GymApp.Core.Models;
using GymApp.Application.Specifications;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymApp.Web.Endpoints
{
    public class GetAllGymActiveSessionsEndpoint : EndpointWithoutRequest<Result<List<SessionDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllGymActiveSessionsEndpoint(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override void Configure()
        {
            Get("/gym/sessions/{gymId}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var gymId = Route<string>("gymId");
            if (string.IsNullOrWhiteSpace(gymId))
            {
                await SendAsync(Result<List<SessionDto>>.Error("Gym ID is required"), cancellation: ct);
                return;
            }

            // Get gym with filtered active sessions
            var gym = await _unitOfWork.ReadRepository<Gym, string>()
                .FirstOrDefaultAsync(new GetAllGymActiveSessionsSpec(gymId));

            if (gym == null)
            {
                await SendAsync(Result<List<SessionDto>>.Error("Gym not found"), cancellation: ct);
                return;
            }

            // Get active sessions
            var sessions = gym.ActiveSessions.ToList();

            var dtos = sessions.Select(s => new SessionDto
            {
                Id = s.Id,
                Name = s.Name,
                UserId = s.UserId,
                CheckInTime = s.CheckInTime,
                LockerId = s.LockerId
            }).ToList();

            await SendAsync(Result<List<SessionDto>>.Success(dtos), cancellation: ct);
        }
    }

    public class SessionDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
        public string? LockerId { get; set; }
    }
}