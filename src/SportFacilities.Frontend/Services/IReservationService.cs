using SportFacilities.Contracts;

namespace SportFacilities.Frontend.Services
{
    public interface IReservationService
    {
        Task<List<ReservationDto>?> GetReservationsAsync(int facilityId);
        Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto);
        Task<bool> DeleteReservationAsync(int reservationId);
        Task<bool> UpdateReservationAsync(int reservationId, UpdateReservationDto dto);
    }
}
