using GatewayService.Services.KeycloakAuthService;
using LibraryService.Dto.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Server.Controllers;

[ApiController]
[Route("api/v1/oauth")]
public class AuthController : ControllerBase
{
    private readonly IKeycloakAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IKeycloakAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Получить JWT токен
    /// </summary>
    /// <remarks>
    /// Endpoint для получения JWT токена через Resource Owner Password Flow.
    /// </remarks>
    /// <param name="request">Учетные данные пользователя</param>
    /// <returns>JWT токен</returns>
    /// <response code="200">Успешная аутентификация, возвращает токен</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="401">Неверные учетные данные</response>
    [HttpPost("token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Authorize([FromForm] AuthorizeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            return BadRequest(new ErrorResponse("Username is required"));

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new ErrorResponse("Password is required"));
        }

        _logger.LogInformation(
            "Authorization request for user {Username}",
            request.Username);

        try
        {
            var tokenResponse = await _authService.AuthenticateAsync(request.Username,
                request.Password,
                request.ClientSecret,
                request.ClientId);

            return Ok(tokenResponse);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex,
                "Failed to authorize user {Username}",
                request.Username);

            return Unauthorized(new ErrorResponse("Invalid username or password"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error during authorization for user {Username}",
                request.Username);

            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorResponse("Internal server error"));
        }
    }
}