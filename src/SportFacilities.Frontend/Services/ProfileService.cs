using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SportFacilities.Contracts;

namespace SportFacilities.Frontend.Services
{
    public class ProfileService : IProfileService
    {
        private readonly HttpClient _httpClient;

        public ProfileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserProfileDto> GetProfileAsync()
        {
            return await _httpClient.GetFromJsonAsync<UserProfileDto>("api/profile");
        }

        public async Task<ApiResponse> ChangePasswordAsync(ChangePasswordDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/profile/changepassword", dto);
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { IsSuccess = true };
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse { IsSuccess = false, ErrorMessage = error };
            }
        }
    }
}
