using GatewayService.Dto.Http;
using LibraryService.Dto.Http;

namespace GatewayService.Services.KeycloakAuthService;

public interface IKeycloakAuthService
{
    public Task<TokenResponse> AuthenticateAsync(string username, string password, string clientSecret, string clientId);
}