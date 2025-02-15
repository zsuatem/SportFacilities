using SportFacilities.Contracts;

namespace SportFacilities.Frontend.Services
{
    public interface ISportFacilityService
    {
        Task<SportFacilityDto> GetFacilityByIdAsync(int id);
        Task<List<SportFacilityDto>> GetAllFacilitiesAsync();
        Task<SportFacilityDto> CreateFacilityAsync(SportFacilityDto facilityDto);
        Task<bool> UpdateFacilityAsync(int id, SportFacilityDto facilityDto);
        Task<bool> DeleteFacilityAsync(int id);
    }
}
