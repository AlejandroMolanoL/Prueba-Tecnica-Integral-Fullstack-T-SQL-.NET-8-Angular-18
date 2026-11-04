using Microsoft.EntityFrameworkCore;
using ComerciantesApp.Domain.Entities;

namespace ComerciantesApp.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Rol> Roles { get; }
    DbSet<Usuario> Usuarios { get; }
    DbSet<Municipio> Municipios { get; }
    DbSet<Comerciante> Comerciantes { get; }
    DbSet<Establecimiento> Establecimientos { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReporteComercianteActivo>> ObtenerReporteComerciantesActivosRawAsync(CancellationToken cancellationToken = default);
}
