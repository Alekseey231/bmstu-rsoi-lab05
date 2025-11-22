using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace LibraryService.Dto.Http;

[DataContract]
public class AuthorizeRequest
{
    [Required]
    [DataMember(Name = "username")]
    public string Username { get; set; }
    
    [Required]
    [DataMember(Name = "password")]
    public string Password { get; set; }

    public AuthorizeRequest(string username, string password)
    {
        Username = username;
        Password = password;
    }
}