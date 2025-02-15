using System.Net.Http.Json;
using SportFacilities.Contracts;

namespace SportFacilities.Frontend.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;
        private readonly ApiAuthenticationStateProvider _authStateProvider;

        public AuthService(HttpClient httpClient, ITokenService tokenService, ApiAuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
            _authStateProvider = authStateProvider;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                if (authResponse != null)
                {
                    await _tokenService.SetTokenAsync(authResponse.Token);
                    _authStateProvider.NotifyUserAuthentication();
                }

                return authResponse;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ApiException(response.StatusCode, errorContent);
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                if (authResponse != null)
                {
                    await _tokenService.SetTokenAsync(authResponse.Token);
                    _authStateProvider.NotifyUserAuthentication();
                }

                return authResponse;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ApiException(response.StatusCode, errorContent);
            }
        }
    }
}
