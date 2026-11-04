using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ComerciantesApp.Application.DTOs.Municipios;
using ComerciantesApp.Application.Interfaces;

namespace ComerciantesApp.Application.Services;

public class MunicipioService : IMunicipioService
{
    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private const string MunicipiosCacheKey = "Municipios_List_CacheKey";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(12);

    public MunicipioService(IApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IReadOnlyList<MunicipioDto>> ObtenerMunicipiosAsync(CancellationToken cancellationToken = default)
    {
        // Reto 06: Implementar Cache en Memoria para evitar accesos adicionales a la base de datos
        if (_cache.TryGetValue(MunicipiosCacheKey, out IReadOnlyList<MunicipioDto>? cachedMunicipios) && cachedMunicipios != null)
        {
            return cachedMunicipios;
        }

        var municipios = await _context.Municipios
            .AsNoTracking()
            .OrderBy(m => m.Departamento)
            .ThenBy(m => m.Nombre)
            .Select(m => new MunicipioDto
            {
                IdMunicipio = m.IdMunicipio,
                Nombre = m.Nombre,
                Departamento = m.Departamento,
                CodigoDANE = m.CodigoDANE
            })
            .ToListAsync(cancellationToken);

        _cache.Set(MunicipiosCacheKey, (IReadOnlyList<MunicipioDto>)municipios, CacheDuration);

        return municipios;
    }
}
