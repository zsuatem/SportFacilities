using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SportFacilities.Frontend.Services;

namespace SportFacilities.Frontend;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // Konfiguracja HttpClient bez autoryzacji (np. do logowania i rejestracji)
        builder.Services.AddScoped(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var baseUrl = config["Api:BaseUrl"] ?? "https://localhost:7204/";
            return new HttpClient { BaseAddress = new Uri(baseUrl) };
        });

        // Dodanie Blazored.LocalStorage
        builder.Services.AddBlazoredLocalStorage();

        // Dodanie MudBlazor
        builder.Services.AddMudServices();

        // Rejestracja usług aplikacji
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ISportFacilityService, SportFacilityService>();
        builder.Services.AddScoped<IReservationService, ReservationService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IProfileService, ProfileService>();

        // Rejestracja AuthenticationStateProvider
        builder.Services.AddScoped<ApiAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<ApiAuthenticationStateProvider>());
        builder.Services.AddAuthorizationCore();

        // Rejestracja delegującego handlera
        builder.Services.AddTransient<JwtExpiredHandler>();

        // Rejestracja HttpClient z AuthorizationMessageHandler
        builder.Services.AddTransient<AuthorizationMessageHandler>();

        builder.Services.AddHttpClient("AuthorizedClient", client =>
            {
                var config = builder.Configuration["Api:BaseUrl"] ?? "https://localhost:7204/";
                client.BaseAddress = new Uri(config);
            })
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .AddHttpMessageHandler<JwtExpiredHandler>();

        // Rejestracja domyślnego HttpClient jako "AuthorizedClient"
        builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthorizedClient"));

        await builder.Build().RunAsync();
    }
}
