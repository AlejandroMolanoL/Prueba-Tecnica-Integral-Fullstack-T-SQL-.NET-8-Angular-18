using ComerciantesApp.Application.Common;
using ComerciantesApp.Application.DTOs.Comerciantes;

namespace ComerciantesApp.Application.Interfaces;

public interface IComercianteService
{
    Task<PagedResult<ComercianteDto>> ObtenerPaginadoAsync(ComercianteFiltrosDto filtros, CancellationToken cancellationToken = default);
    Task<ComercianteDto> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ComercianteDto> CrearAsync(CrearComercianteDto dto, CancellationToken cancellationToken = default);
    Task<ComercianteDto> ActualizarAsync(int id, ActualizarComercianteDto dto, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    Task<ComercianteDto> ModificarEstadoAsync(int id, string nuevoEstado, CancellationToken cancellationToken = default);
}
