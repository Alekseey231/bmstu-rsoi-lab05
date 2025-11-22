using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace LibraryService.Dto.Http;

public class AuthorizeRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    
    public string GrantType { get; set; }
    public string Scope { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}
