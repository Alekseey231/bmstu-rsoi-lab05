namespace LibrarySystem.Helpers.Auth.Services;

public interface IUserService
{
    string? GetUserId();
    string? GetUsername();
    string? GetEmail();
    bool IsAuthenticated();
    IEnumerable<string> GetRoles();
}