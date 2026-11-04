using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ComerciantesApp.Application.Common;
using ComerciantesApp.Application.DTOs.Auth;
using ComerciantesApp.Application.Interfaces;

namespace ComerciantesApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Realiza la autenticación (login) del usuario y retorna un token JWT válido por 1 hora. (Reto 05)
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, cancellationToken);
        return Ok(ApiResponse<LoginResponseDto>.SuccessResult(response, "Inicio de sesión exitoso."));
    }
}
