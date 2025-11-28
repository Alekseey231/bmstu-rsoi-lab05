using GatewayService.Dto.Http;
using LibraryService.Dto.Http;
using Refit;

namespace GatewayService.Clients;

public interface IKeycloakClient
{
    [Post("/protocol/openid-connect/token")]
    [Headers("Content-Type: application/x-www-form-urlencoded")]
    Task<TokenResponse> GetTokenAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> formData);
}