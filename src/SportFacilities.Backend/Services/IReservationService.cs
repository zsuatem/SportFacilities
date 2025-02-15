using SportFacilities.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportFacilities.Backend.Services
{
    public interface IReservationService
    {
        Task<List<ReservationDto>> GetReservationsByFacilityIdAsync(int facilityId);
        Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto, string userId);
        Task<ReservationDto> UpdateReservationAsync(int reservationId, UpdateReservationDto dto, string userId);
        Task<bool> DeleteReservationAsync(int reservationId, string userId);
        Task<List<ReservationDto>> GetAllReservationsAsync();
    }
}
