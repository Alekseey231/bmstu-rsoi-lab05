using Microsoft.Extensions.DependencyInjection;

namespace LibrarySystem.Helpers.Auth.Services.Extensions;

public static class ProviderExtensions
{
    public static void AddUserService(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
    }
}