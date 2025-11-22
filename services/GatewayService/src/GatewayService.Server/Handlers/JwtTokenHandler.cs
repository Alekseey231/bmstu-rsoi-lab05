using System.Net.Http.Headers;

namespace GatewayService.Server.Handlers;

public class JwtTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<JwtTokenHandler> _logger;

    public JwtTokenHandler(
        IHttpContextAccessor httpContextAccessor,
        ILogger<JwtTokenHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();

        if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
        {
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(token);
            
            _logger.LogDebug("Forwarding JWT token to {Uri}", request.RequestUri);
        }
        else
        {
            _logger.LogWarning("No JWT token found in request to {Uri}", request.RequestUri);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}