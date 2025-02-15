using SportFacilities.Backend.Data;
using SportFacilities.Backend.Entities;
using SportFacilities.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SportFacilities.Backend.Services
{
    public class SportFacilityService : ISportFacilityService
    {
        private readonly AppDbContext _context;

        public SportFacilityService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SportFacilityDto>> GetAllFacilitiesAsync()
        {
            return await _context.SportFacilities
                .Include(f => f.Availabilities)
                .Select(f => new SportFacilityDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Location = f.Location,
                    Description = f.Description,
                    Availabilities = f.Availabilities.Select(a => new FacilityAvailabilityDto
                    {
                        DayOfWeek = a.DayOfWeek,
                        IsAvailable = a.IsAvailable,
                        OpeningTime = a.OpeningTime,
                        ClosingTime = a.ClosingTime
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<SportFacilityDto> GetFacilityByIdAsync(int id)
        {
            var facility = await _context.SportFacilities
                .Include(f => f.Availabilities)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (facility == null) return null;

            return new SportFacilityDto
            {
                Id = facility.Id,
                Name = facility.Name,
                Location = facility.Location,
                Description = facility.Description,
                Availabilities = facility.Availabilities.Select(a => new FacilityAvailabilityDto
                {
                    DayOfWeek = a.DayOfWeek,
                    IsAvailable = a.IsAvailable,
                    OpeningTime = a.OpeningTime,
                    ClosingTime = a.ClosingTime
                }).ToList()
            };
        }

        public async Task<SportFacilityDto> CreateFacilityAsync(SportFacilityDto facilityDto)
        {
            var facility = new SportFacility
            {
                Name = facilityDto.Name,
                Location = facilityDto.Location,
                Description = facilityDto.Description,
                Availabilities = facilityDto.Availabilities.Select(a => new FacilityAvailability
                {
                    DayOfWeek = a.DayOfWeek,
                    IsAvailable = a.IsAvailable,
                    OpeningTime = a.IsAvailable ? a.OpeningTime : (TimeSpan?)null,
                    ClosingTime = a.IsAvailable ? a.ClosingTime : (TimeSpan?)null
                }).ToList()
            };

            _context.SportFacilities.Add(facility);
            await _context.SaveChangesAsync();

            facilityDto.Id = facility.Id;
            return facilityDto;
        }

        public async Task<bool> UpdateFacilityAsync(int id, SportFacilityDto facilityDto)
        {
            var facility = await _context.SportFacilities
                .Include(f => f.Availabilities)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (facility == null) return false;

            facility.Name = facilityDto.Name;
            facility.Location = facilityDto.Location;
            facility.Description = facilityDto.Description;
            
            foreach (var availabilityDto in facilityDto.Availabilities)
            {
                var availability = facility.Availabilities.FirstOrDefault(a => a.DayOfWeek == availabilityDto.DayOfWeek);
                if (availability == null)
                {
                    availability = new FacilityAvailability
                    {
                        DayOfWeek = availabilityDto.DayOfWeek,
                        IsAvailable = availabilityDto.IsAvailable,
                        OpeningTime = availabilityDto.IsAvailable ? availabilityDto.OpeningTime : (TimeSpan?)null,
                        ClosingTime = availabilityDto.IsAvailable ? availabilityDto.ClosingTime : (TimeSpan?)null
                    };
                    facility.Availabilities.Add(availability);
                }
                else
                {
                    availability.IsAvailable = availabilityDto.IsAvailable;
                    availability.OpeningTime = availabilityDto.IsAvailable ? availabilityDto.OpeningTime : (TimeSpan?)null;
                    availability.ClosingTime = availabilityDto.IsAvailable ? availabilityDto.ClosingTime : (TimeSpan?)null;
                }
            }

            _context.SportFacilities.Update(facility);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteFacilityAsync(int id)
        {
            var facility = await _context.SportFacilities.FindAsync(id);
            if (facility == null) return false;

            _context.SportFacilities.Remove(facility);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<ReservationDto>> GetReservationsByFacilityIdAsync(int facilityId)
        {
            return await _context.Reservations
                .Where(r => r.SportFacilityId == facilityId)
                .Include(r => r.User)
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    SportFacilityId = r.SportFacilityId,
                    UserId = r.UserId,
                    UserEmail = r.User.Email,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    Description = r.Description
                })
                .ToListAsync();
        }
    }
}
