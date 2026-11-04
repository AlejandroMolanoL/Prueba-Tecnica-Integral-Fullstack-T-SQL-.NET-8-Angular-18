using ComerciantesApp.Application.DTOs.Reportes;

namespace ComerciantesApp.Application.Interfaces;

public interface IReporteService
{
    Task<IReadOnlyList<ReporteComercianteDto>> ObtenerDatosReporteActivosAsync(CancellationToken cancellationToken = default);
    Task<byte[]> GenerarReporteCsvPipeAsync(CancellationToken cancellationToken = default);
}
