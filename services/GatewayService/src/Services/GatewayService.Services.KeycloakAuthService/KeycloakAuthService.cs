using GatewayService.Clients;
using LibraryService.Dto.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Refit;

namespace GatewayService.Services.KeycloakAuthService;

public class KeycloakAuthService : IKeycloakAuthService
{
    private readonly IKeycloakClient _keycloakClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<KeycloakAuthService> _logger;

    public KeycloakAuthService(
        IKeycloakClient keycloakClient,
        IConfiguration configuration,
        ILogger<KeycloakAuthService> logger)
    {
        _keycloakClient = keycloakClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<TokenResponse> AuthenticateAsync(string username, string password, string clientSecret, string clientId)
    {
        _logger.LogInformation("Authenticating user {Username} with Keycloak", username);

        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["username"] = username,
            ["password"] = password,
            ["scope"] = "openid profile email"
        };

        try
        {
            var tokenResponse = await _keycloakClient.GetTokenAsync(formData);
            
            _logger.LogInformation("Successfully authenticated user {Username}", username);
            
            return tokenResponse;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex,
                "Keycloak authentication failed for user {Username}: {StatusCode}",
                username, ex.StatusCode);

            throw new HttpRequestException(
                $"Authentication failed: {ex.StatusCode}", ex);
        }
    }
}