using Ardalis.Result;
using FastEndpoints;
using GymApp.Core.Enums;
using GymApp.Core.Interfaces;
using GymApp.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace GymApp.Web.Endpoints
{
    public class CreateGymEndpoint : Endpoint<CreateGymEndpointRequest, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateGymEndpoint(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override void Configure()
        {
            Post("/gyms");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CreateGymEndpointRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.Name) || string.IsNullOrWhiteSpace(req.Address) || string.IsNullOrWhiteSpace(req.PhoneNumber))
            {
                await SendAsync(Result<string>.Error("Name, Address, and PhoneNumber are required"), cancellation: ct);
                return;
            }

            if (!TimeOnly.TryParse(req.OpeningTime, out var openingTime) || !TimeOnly.TryParse(req.ClosingTime, out var closingTime))
            {
                await SendAsync(Result<string>.Error("Invalid OpeningTime or ClosingTime format"), cancellation: ct);
                return;
            }

            if (!Enum.TryParse<GenderEnum>(req.GymGender, true, out var gymGender))
            {
                await SendAsync(Result<string>.Error("Invalid GymGender value"), cancellation: ct);
                return;
            }

            if (req.LockerCount < 0)
            {
                await SendAsync(Result<string>.Error("LockerCount cannot be negative"), cancellation: ct);
                return;
            }

            var gymId = Guid.NewGuid().ToString();
            var gym = new Gym(gymId, req.Name, req.Address, req.PhoneNumber, gymGender, openingTime, closingTime, req.LockerCount);

            await _unitOfWork.WriteRepository<Gym, string>().AddAsync(gym);
            await _unitOfWork.SaveChangesAsync(ct);

            await SendAsync(Result<string>.Success($"Gym '{gymId}' created successfully!"), cancellation: ct);
        }
    }

    public class CreateGymEndpointRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string GymGender { get; set; } = string.Empty; // e.g., "Male", "Female", "Mixed"
        public string OpeningTime { get; set; } = string.Empty; // e.g., "06:00"
        public string ClosingTime { get; set; } = string.Empty; // e.g., "22:00"
        public int LockerCount { get; set; }
    }
}