using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ComerciantesApp.Application.Common;
using ComerciantesApp.Application.DTOs.Comerciantes;
using ComerciantesApp.Application.Interfaces;
using ComerciantesApp.Domain.Constants;

namespace ComerciantesApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Privado: requiere token JWT para su ejecucion (Reto 07 y Reto 08)
public class ComerciantesController : ControllerBase
{
    private readonly IComercianteService _comercianteService;
    private readonly IReporteService _reporteService;

    public ComerciantesController(
        IComercianteService comercianteService,
        IReporteService reporteService)
    {
        _comercianteService = comercianteService;
        _reporteService = reporteService;
    }

    /// <summary>
    /// Consulta paginada de comerciantes con filtros (5 registros por página por defecto). (Reto 07)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ComercianteDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerPaginado([FromQuery] ComercianteFiltrosDto filtros, CancellationToken cancellationToken)
    {
        var resultado = await _comercianteService.ObtenerPaginadoAsync(filtros, cancellationToken);
        return Ok(ApiResponse<PagedResult<ComercianteDto>>.SuccessResult(resultado, "Consulta de comerciantes exitosa."));
    }

    /// <summary>
    /// Consulta un comerciante por su identificador único. (Reto 07)
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ComercianteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(int id, CancellationToken cancellationToken)
    {
        var comerciante = await _comercianteService.ObtenerPorIdAsync(id, cancellationToken);
        return Ok(ApiResponse<ComercianteDto>.SuccessResult(comerciante, "Comerciante encontrado exitosamente."));
    }

    /// <summary>
    /// Crea un nuevo comerciante. Modifica los campos de auditoría automáticamente. (Reto 07)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ComercianteDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear([FromBody] CrearComercianteDto dto, CancellationToken cancellationToken)
    {
        var comerciante = await _comercianteService.CrearAsync(dto, cancellationToken);
        return CreatedAtAction(
            nameof(ObtenerPorId), 
            new { id = comerciante.IdComerciante }, 
            ApiResponse<ComercianteDto>.SuccessResult(comerciante, "Comerciante creado exitosamente."));
    }

    /// <summary>
    /// Actualiza la información de un comerciante existente. (Reto 07)
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ComercianteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarComercianteDto dto, CancellationToken cancellationToken)
    {
        var comerciante = await _comercianteService.ActualizarAsync(id, dto, cancellationToken);
        return Ok(ApiResponse<ComercianteDto>.SuccessResult(comerciante, "Comerciante actualizado exitosamente."));
    }

    /// <summary>
    /// Elimina un comerciante por su Id. Solo accesible para el rol Administrador. (Reto 07)
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = RolesConstants.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken)
    {
        var resultado = await _comercianteService.EliminarAsync(id, cancellationToken);
        return Ok(ApiResponse<bool>.SuccessResult(resultado, "Comerciante eliminado exitosamente."));
    }

    /// <summary>
    /// Modifica el estado del comerciante (Activo / Inactivo) mediante HTTP PATCH. (Reto 07)
    /// </summary>
    [HttpPatch("{id:int}/estado")]
    [ProducesResponseType(typeof(ApiResponse<ComercianteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ModificarEstado(int id, [FromBody] ActualizarEstadoDto dto, CancellationToken cancellationToken)
    {
        var comerciante = await _comercianteService.ModificarEstadoAsync(id, dto.Estado, cancellationToken);
        return Ok(ApiResponse<ComercianteDto>.SuccessResult(comerciante, "Estado del comerciante actualizado exitosamente."));
    }

    /// <summary>
    /// Genera y descarga un archivo plano (.csv) delimitado por pipe (|) de comerciantes activos. Solo accesible para el rol Administrador. (Reto 08)
    /// </summary>
    [HttpGet("reporte-csv")]
    [Authorize(Roles = RolesConstants.Administrador)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DescargarReporteCsv(CancellationToken cancellationToken)
    {
        var fileBytes = await _reporteService.GenerarReporteCsvPipeAsync(cancellationToken);
        var fileName = $"Reporte_Comerciantes_Activos_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

        return File(fileBytes, "text/csv; charset=utf-8", fileName);
    }
}

