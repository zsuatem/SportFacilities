using SportFacilities.Contracts;
using System.Net.Http.Json;

namespace SportFacilities.Frontend.Services
{
    public class ReservationService : IReservationService
    {
        private readonly HttpClient _httpClient;

        public ReservationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ReservationDto>> GetReservationsAsync(int facilityId)
        {
            var response = await _httpClient.GetAsync($"api/facilities/{facilityId}/reservations");
            if (response.IsSuccessStatusCode)
            {
                var reservations = await response.Content.ReadFromJsonAsync<List<ReservationDto>>();
                return reservations;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ApiException(response.StatusCode, errorContent);
            }
        }

        public async Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/reservations", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ReservationDto>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ApiException(response.StatusCode, errorContent);
            }
        }


        public async Task<bool> DeleteReservationAsync(int reservationId)
        {
            var response = await _httpClient.DeleteAsync($"api/reservations/{reservationId}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ApiException(response.StatusCode, errorContent);
            }
        }

        public async Task<bool> UpdateReservationAsync(int reservationId, UpdateReservationDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/reservations/{reservationId}", dto);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ApiException(response.StatusCode, errorContent);
            }
        }
    }
}
