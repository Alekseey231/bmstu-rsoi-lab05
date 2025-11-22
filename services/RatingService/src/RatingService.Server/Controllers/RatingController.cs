using System.ComponentModel.DataAnnotations;
using LibrarySystem.Helpers.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RatingService.Core.Interfaces;
using RatingService.Dto.Http;
using RatingService.Dto.Http.Converters;
using RatingService.Dto.Http.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace RatingService.Server.Controllers;


[ApiController]
[Route("/api/v1/rating")]
[Authorize]
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;
    private readonly IUserService _userService;
    private readonly ILogger<RatingController> _logger;

    public RatingController(IRatingService ratingService, 
        IUserService userService,
        ILogger<RatingController> logger)
    {
        _ratingService = ratingService;
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    [SwaggerOperation("Метод для получения рейтинга пользователя.", "Метод для получения рейтинга пользователя.")]
    [SwaggerResponse(statusCode: 200, type: typeof(Rating), description: "Рейтинг успешно получен.")]
    [SwaggerResponse(statusCode: 400, type: typeof(ErrorResponse), description: "Отсутствует заголовок X-User-Name.")]
    [SwaggerResponse(statusCode: 500, type: typeof(ErrorResponse), description: "Ошибка на стороне сервера.")]
    public async Task<IActionResult> GetRating()
    {
        try
        {
            var username = _userService.GetUsername();
            
            if (username is null)
                return BadRequest("username is required");
            
            var result = await _ratingService.GetRatingByUserNameAsync(username);
            
            var dtoRating = RatingConverter.Convert(result);
            
            return Ok(dtoRating);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in method {MethodName}.", nameof(GetRating));
            
            return StatusCode(500, e.Message);
        }    
    }
    
    [HttpPatch]
    [SwaggerOperation("Метод для обновления рейтинга пользователя.", "Метод для обновления рейтинга пользователя.")]
    [SwaggerResponse(statusCode: 200, type: typeof(Rating), description: "Рейтинг успешно обновлен.")]
    [SwaggerResponse(statusCode: 400, type: typeof(ErrorResponse), description: "Одно или несколько полей модели невалидны.")]
    [SwaggerResponse(statusCode: 500, type: typeof(ErrorResponse), description: "Ошибка на стороне сервера.")]
    public async Task<IActionResult> UpdateRating([Required][FromBody] UpdateRatingRequest request)
    {
        try
        {
            var username = _userService.GetUsername();
            
            if (username is null)
                return BadRequest("username is required");

            var result = await _ratingService.UpdateRatingAsync(username, request.Stars);
            
            var dtoRating = RatingConverter.Convert(result);
            
            return Ok(dtoRating);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in method {MethodName}.", nameof(UpdateRating));
            
            return StatusCode(500, e.Message);
        }    
    }
}