using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ComerciantesApp.Application.Common;
using ComerciantesApp.Application.DTOs.Municipios;
using ComerciantesApp.Application.Interfaces;

namespace ComerciantesApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Privado: requiere token JWT (Reto 06)
public class MunicipiosController : ControllerBase
{
    private readonly IMunicipioService _municipioService;

    public MunicipiosController(IMunicipioService municipioService)
    {
        _municipioService = municipioService;
    }

    /// <summary>
    /// Retorna la lista de valores para el campo Municipio con soporte de caché en memoria. (Reto 06)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<MunicipioDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ObtenerMunicipios(CancellationToken cancellationToken)
    {
        var municipios = await _municipioService.ObtenerMunicipiosAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<MunicipioDto>>.SuccessResult(municipios, "Municipios recuperados exitosamente."));
    }
}

