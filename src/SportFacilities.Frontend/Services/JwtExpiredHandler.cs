using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace SportFacilities.Frontend.Services
{
    public class JwtExpiredHandler : DelegatingHandler
    {
        private readonly NavigationManager _navigationManager;
        private readonly ITokenService _tokenService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public JwtExpiredHandler(NavigationManager navigationManager, ITokenService tokenService, AuthenticationStateProvider authenticationStateProvider)
        {
            _navigationManager = navigationManager;
            _tokenService = tokenService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenService.RemoveTokenAsync();
                if (_authenticationStateProvider is ApiAuthenticationStateProvider authStateProvider)
                {
                    authStateProvider.NotifyUserLogout();
                }

                _navigationManager.NavigateTo("/login", true);
            }

            return response;
        }
    }
}
