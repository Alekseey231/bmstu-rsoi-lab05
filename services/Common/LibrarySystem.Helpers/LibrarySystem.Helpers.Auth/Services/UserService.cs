using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace LibrarySystem.Helpers.Auth.Services;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? GetUserId()
    {
        return User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? User?.FindFirst("sub")?.Value;
    }

    public string? GetUsername()
    {
        return User?.FindFirst(ClaimTypes.Name)?.Value
               ?? User?.FindFirst("preferred_username")?.Value
               ?? User?.FindFirst("name")?.Value;
    }

    public string? GetEmail()
    {
        return User?.FindFirst(ClaimTypes.Email)?.Value
               ?? User?.FindFirst("email")?.Value;
    }

    public bool IsAuthenticated()
    {
        return User?.Identity?.IsAuthenticated ?? false;
    }

    public IEnumerable<string> GetRoles()
    {
        return User?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? [];
    }
}