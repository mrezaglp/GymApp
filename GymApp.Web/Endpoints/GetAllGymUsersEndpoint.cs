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
    public class GetAllGymUsersEndpoint : EndpointWithoutRequest<Result<List<UserDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllGymUsersEndpoint(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override void Configure()
        {
            Get("/gym/users/{gymId}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var gymId = Route<string>("gymId");
            if (string.IsNullOrWhiteSpace(gymId))
            {
                await SendAsync(Result<List<UserDto>>.Error("Gym ID is required"), cancellation: ct);
                return;
            }

            // Get gym with filtered memberships and users
            var gym = await _unitOfWork.ReadRepository<Gym, string>()
                .FirstOrDefaultAsync(new GetAllGymUsersSpec(gymId));

            if (gym == null)
            {
                await SendAsync(Result<List<UserDto>>.Error("Gym not found"), cancellation: ct);
                return;
            }

            // Get users from active memberships
            var users = gym.Memberships.Select(m => m.User).Distinct().ToList();

            var dtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                UserStatus = u.UserStatus.ToString()
            }).ToList();

            await SendAsync(Result<List<UserDto>>.Success(dtos), cancellation: ct);
        }
    }

    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string UserStatus { get; set; } = string.Empty;
    }
}