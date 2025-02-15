using SportFacilities.Contracts;

namespace SportFacilities.Backend.Services;

public interface ISportFacilityService
{
    Task<List<SportFacilityDto>> GetAllFacilitiesAsync();
    Task<SportFacilityDto> GetFacilityByIdAsync(int id);
    Task<SportFacilityDto> CreateFacilityAsync(SportFacilityDto facilityDto);
    Task<bool> UpdateFacilityAsync(int id, SportFacilityDto facilityDto);
    Task<bool> DeleteFacilityAsync(int id);
    Task<List<ReservationDto>> GetReservationsByFacilityIdAsync(int facilityId);
}
