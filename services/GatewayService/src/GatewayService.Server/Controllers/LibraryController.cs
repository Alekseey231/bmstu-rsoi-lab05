using System.ComponentModel.DataAnnotations;
using GatewayService.Clients;
using GatewayService.Dto.Http;
using GatewayService.Dto.Http.Converters;
using GatewayService.Services.CircuitBreaker.Exceptions;
using LibrarySystem.Helpers.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GatewayService.Server.Controllers;

[ApiController]
[Route("/api/v1/libraries")]
[Authorize]
public class LibraryController : ControllerBase
{
    private readonly ILibraryServiceClient _libraryServiceRequestClient;
    private readonly IUserService _userService;
    private readonly ILogger<LibraryController> _logger;

    public LibraryController(ILibraryServiceClient libraryServiceRequestClient, 
        IUserService userService,
        ILogger<LibraryController> logger)
    {
        _libraryServiceRequestClient = libraryServiceRequestClient;
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    [SwaggerOperation("Получить список библиотек в городе", "Получить список библиотек в городе")]
    [SwaggerResponse(statusCode: 200, type: typeof(LibraryPaginationResponse), description: "Список библиотек в городе")]
    [SwaggerResponse(statusCode: 500, type: typeof(ErrorResponse), description: "Ошибка на стороне сервера")]
    public async Task<IActionResult> GetLibraries([Required][FromQuery] string city,
        [FromQuery] int? page,
        [FromQuery] int? size)
    {
        try
        {
            var username = _userService.GetUsername();
            
            _logger.LogDebug("User {Username} requesting libraries in city {City}", username, city);
            
            var libraries = await _libraryServiceRequestClient.GetLibrariesAsync(city, page, size);

            var dtoLibraries = LibraryResponseConverter.Convert(libraries);
            
            _logger.LogInformation("User {Username} successfully got {Count} libraries in city {City}", username, libraries.Items.Count, city);

            return Ok(dtoLibraries);
        }
        catch (BrokenCircuitException e)
        {
            _logger.LogError(e, "Library service unavailable");
            
            return StatusCode(503, new ErrorResponse("Library Service unavailable."));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in method {Method}", nameof(GetLibraries));
            
            return StatusCode(500, new ErrorResponse("Неожиданная ошибка на стороне сервера."));
        }
    }

    [HttpGet("{libraryUid:guid}/books")]
    [SwaggerOperation("Получить список книг в выбранной библиотеке", "Получить список книг в выбранной библиотеке")]
    [SwaggerResponse(statusCode: 200, type: typeof(LibraryBookPaginationResponse), description: "Список книг в библиотеке")]
    [SwaggerResponse(statusCode: 500, type: typeof(ErrorResponse), description: "Ошибка на стороне сервера")]
    public async Task<IActionResult> GetLibraryBooks([Required][FromRoute] Guid libraryUid,
        [FromQuery] int? page,
        [FromQuery] int? size,
        [FromQuery] bool? showAll)
    {
        try
        {
            var books = await _libraryServiceRequestClient.GetBooksAsync(libraryUid,
                showAll,
                page,
                size);
            
            var dtoBooks = LibraryBookPaginationResponseConverter.Convert(books);
            
            return Ok(dtoBooks);
        }
        catch (BrokenCircuitException e)
        {
            _logger.LogError(e, "Library service unavailable");
            
            return StatusCode(503, new ErrorResponse("Library Service unavailable."));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in method {Method}", nameof(GetLibraryBooks));
            
            return StatusCode(500, new ErrorResponse("Неожиданная ошибка на стороне сервера."));
        }
    }
}
