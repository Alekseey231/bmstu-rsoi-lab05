using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace GatewayService.Dto.Http;

[DataContract]
public class TokenResponse
{
    [Required]
    [JsonPropertyName("access_token")]
    [DataMember(Name = "access_token")]
    public string AccessToken { get; set; }
    
    [Required]
    [JsonPropertyName("token_type")]
    [DataMember(Name = "token_type")]
    public string TokenType { get; set; } = "Bearer";

    [Required]
    [JsonPropertyName("expires_in")]
    [DataMember(Name = "expires_in")]
    public int ExpiresIn { get; set; }
    
    [JsonPropertyName("refresh_token")]
    [DataMember(Name = "refresh_token")]
    public string? RefreshToken { get; set; }

    [JsonPropertyName("scope")]
    [DataMember(Name = "scope")]
    public string? Scope { get; set; }
}