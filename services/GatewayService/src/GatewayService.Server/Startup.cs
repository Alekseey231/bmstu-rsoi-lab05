using System.Reflection;
using GatewayService.Clients;
using GatewayService.Server.Extensions;
using GatewayService.Server.Handlers;
using GatewayService.Services.KeycloakAuthService;
using GatewayService.Services.RequestsProcessingBackgroundService.Extensions;
using GatewayService.Services.RequestsQueue.Extensions;
using LibrarySystem.Helpers.Auth.Extensions;
using LibrarySystem.Helpers.Auth.Services.Extensions;
using Microsoft.OpenApi.Models;
using Refit;

namespace GatewayService.Server;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<JwtTokenHandler>();
        
        services.AddHttpContextAccessor();
        services.AddKeycloakAuthentication(Configuration);
        services.AddUserService();
        
        services.AddRequestsBackgroundServices();
        services.AddRequestsQueues();
        
        var keycloakBaseUrl = Configuration["Authentication:Authority"]!;
        services.AddRefitClient<IKeycloakClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(keycloakBaseUrl));
        services.AddScoped<IKeycloakAuthService, KeycloakAuthService>();
        
        services.AddRefitClients(Configuration);
        services.AddControllers().AddNewtonsoftJson();
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "GatewayService.Server", Version = "v1" });
    
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // ← важно: строчными буквами
                BearerFormat = "JWT"
            });
    
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        services.AddSwaggerGenNewtonsoftSupport();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger(c =>
        {
            c.RouteTemplate = "/api/v1/swagger/{documentName}/swagger.json";
        });
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/api/v1/swagger/v1/swagger.json", "GatewayService.Server.Http v1");
            c.RoutePrefix = "api/v1/swagger";
        });
        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}