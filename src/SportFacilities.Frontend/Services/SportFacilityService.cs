using SportFacilities.Contracts;
using System.Net.Http.Json;

namespace SportFacilities.Frontend.Services
{
    public class SportFacilityService : ISportFacilityService
    {
        private readonly HttpClient _httpClient;

        public SportFacilityService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<SportFacilityDto>> GetAllFacilitiesAsync()
        {
            var response = await _httpClient.GetAsync("api/facilities");
            if (response.IsSuccessStatusCode)
            {
                var facilities = await response.Content.ReadFromJsonAsync<List<SportFacilityDto>>();
                return facilities ?? new List<SportFacilityDto>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ApiException(response.StatusCode, errorContent);
            }
        }

        public async Task<SportFacilityDto> GetFacilityByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/facilities/{id}");
            if (response.IsSuccessStatusCode)
            {
                var facility = await response.Content.ReadFromJsonAsync<SportFacilityDto>();
                if (facility == null)
                    throw new ApiException(response.StatusCode, "Obiekt sportowy nie został znaleziony.");

                return facility;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ApiException(response.StatusCode, errorContent);
            }
        }

        public async Task<SportFacilityDto> CreateFacilityAsync(SportFacilityDto facilityDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/facilities", facilityDto);
            if (response.IsSuccessStatusCode)
            {
                var createdFacility = await response.Content.ReadFromJsonAsync<SportFacilityDto>();
                if (createdFacility == null)
                    throw new ApiException(response.StatusCode, "Nie udało się utworzyć obiektu sportowego.");

                return createdFacility;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ApiException(response.StatusCode, errorContent);
            }
        }

        public async Task<bool> UpdateFacilityAsync(int id, SportFacilityDto facilityDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/facilities/{id}", facilityDto);
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

        public async Task<bool> DeleteFacilityAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/facilities/{id}");
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
