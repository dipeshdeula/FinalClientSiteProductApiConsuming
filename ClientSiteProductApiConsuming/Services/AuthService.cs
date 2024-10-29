using ClientSiteProductApiConsuming.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ClientSiteProductApiConsuming.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;

        public AuthService(HttpClient httpClient, ILogger<AuthService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> AuthenticateAsync(UserLogin loginModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("http://localhost:5160/api/Auth/login", loginModel);
                response.EnsureSuccessStatusCode();

                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return tokenResponse?.Token;

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "An error occurred while authenticating the user.");
                return null;
            }
           

        }
    }
}
