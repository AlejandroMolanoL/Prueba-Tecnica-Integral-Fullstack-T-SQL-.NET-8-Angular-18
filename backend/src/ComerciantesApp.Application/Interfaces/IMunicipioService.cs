using ComerciantesApp.Application.DTOs.Municipios;

namespace ComerciantesApp.Application.Interfaces;

public interface IMunicipioService
{
    Task<IReadOnlyList<MunicipioDto>> ObtenerMunicipiosAsync(CancellationToken cancellationToken = default);
}

