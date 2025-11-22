using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace LibraryService.Dto.Http;

[DataContract]
public class TokenResponse
{
    [Required]
    [DataMember(Name = "access_token")]
    public string AccessToken { get; set; }
    
    [Required]
    [DataMember(Name = "token_type")]
    public string TokenType { get; set; } = "Bearer";

    [Required]
    [DataMember(Name = "expires_in")]
    public int ExpiresIn { get; set; }
    
    [DataMember(Name = "refresh_token")]
    public string? RefreshToken { get; set; }

    [DataMember(Name = "scope")]
    public string? Scope { get; set; }
}