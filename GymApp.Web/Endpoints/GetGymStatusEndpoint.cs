using Ardalis.Result;
using FastEndpoints;
using GymApp.Core.Interfaces;
using GymApp.Core.Models;
using GymApp.Application.Specifications;
using System.Threading;
using System.Threading.Tasks;

namespace GymApp.Web.Endpoints
{
    public class GetGymOccupancyEndpoint : EndpointWithoutRequest<Result<GymOccupancyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetGymOccupancyEndpoint(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override void Configure()
        {
            Get("/gyms/{id}/occupancy");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var gymId = Route<string>("id");
            if (string.IsNullOrWhiteSpace(gymId))
            {
                await SendAsync(Result<GymOccupancyDto>.Error("Gym ID is required"), cancellation: ct);
                return;
            }

            // Get gym
            var gym = await _unitOfWork.ReadRepository<Gym, string>().GetByIdAsync(gymId);
            if (gym == null)
            {
                await SendAsync(Result<GymOccupancyDto>.Error("Gym not found"), cancellation: ct);
                return;
            }

            // Count active sessions
            var sessionSpec = new ActiveSessionsSpec(gymId);
            var activeSessionsCount = await _unitOfWork.ReadRepository<GymSession, string>().CountAsync(sessionSpec);

            // Calculate occupancy percentage
            decimal occupancyPercentage = gym.LockerCount > 0
                ? (decimal)activeSessionsCount / gym.LockerCount * 100
                : 0;

            var dto = new GymOccupancyDto
            {
                GymId = gym.Id,
                GymName = gym.Name,
                ActiveSessionsCount = activeSessionsCount,
                TotalLockers = gym.LockerCount,
                OccupancyPercentage = Math.Round(occupancyPercentage, 2)
            };

            await SendAsync(Result<GymOccupancyDto>.Success(dto), cancellation: ct);
        }
    }

    public class GymOccupancyDto
    {
        public string GymId { get; set; } = string.Empty;
        public string GymName { get; set; } = string.Empty;
        public int ActiveSessionsCount { get; set; }
        public int TotalLockers { get; set; }
        public decimal OccupancyPercentage { get; set; }
    }
}