using Microsoft.EntityFrameworkCore;
using ComerciantesApp.Application.Common;
using ComerciantesApp.Application.DTOs.Comerciantes;
using ComerciantesApp.Application.Exceptions;
using ComerciantesApp.Application.Interfaces;
using ComerciantesApp.Domain.Entities;
using FluentValidation;

namespace ComerciantesApp.Application.Services;

public class ComercianteService : IComercianteService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<CrearComercianteDto> _crearValidator;
    private readonly IValidator<ActualizarComercianteDto> _actualizarValidator;
    private readonly IValidator<ActualizarEstadoDto> _estadoValidator;

    public ComercianteService(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IValidator<CrearComercianteDto> crearValidator,
        IValidator<ActualizarComercianteDto> actualizarValidator,
        IValidator<ActualizarEstadoDto> estadoValidator)
    {
        _context = context;
        _currentUserService = currentUserService;
        _crearValidator = crearValidator;
        _actualizarValidator = actualizarValidator;
        _estadoValidator = estadoValidator;
    }

    public async Task<PagedResult<ComercianteDto>> ObtenerPaginadoAsync(ComercianteFiltrosDto filtros, CancellationToken cancellationToken = default)
    {
        var query = _context.Comerciantes
            .Include(c => c.Municipio)
            .Include(c => c.Establecimientos)
            .AsNoTracking()
            .AsQueryable();

        // Filtros (Reto 07)
        if (!string.IsNullOrWhiteSpace(filtros.NombreRazonSocial))
        {
            var search = filtros.NombreRazonSocial.Trim().ToLower();
            query = query.Where(c => c.NombreRazonSocial.ToLower().Contains(search));
        }

        if (filtros.FechaRegistro.HasValue)
        {
            var fecha = filtros.FechaRegistro.Value.Date;
            query = query.Where(c => c.FechaRegistro.Date == fecha);
        }

        if (!string.IsNullOrWhiteSpace(filtros.Estado))
        {
            var estado = filtros.Estado.Trim();
            query = query.Where(c => c.Estado == estado);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var pageNumber = filtros.PageNumber < 1 ? 1 : filtros.PageNumber;
        var pageSize = filtros.PageSize < 1 ? 5 : filtros.PageSize;

        var items = await query
            .OrderByDescending(c => c.IdComerciante)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ComercianteDto
            {
                IdComerciante = c.IdComerciante,
                NombreRazonSocial = c.NombreRazonSocial,
                IdMunicipio = c.IdMunicipio,
                MunicipioNombre = c.Municipio != null ? c.Municipio.Nombre : string.Empty,
                Telefono = c.Telefono,
                CorreoElectronico = c.CorreoElectronico,
                FechaRegistro = c.FechaRegistro,
                Estado = c.Estado,
                FechaActualizacion = c.FechaActualizacion,
                UsuarioActualizacion = c.UsuarioActualizacion,
                CantidadEstablecimientos = c.Establecimientos.Count,
                TotalIngresos = c.Establecimientos.Sum(e => e.Ingresos),
                CantidadEmpleados = c.Establecimientos.Sum(e => e.NumeroEmpleados)
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ComercianteDto>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<ComercianteDto> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var comerciante = await _context.Comerciantes
            .Include(c => c.Municipio)
            .Include(c => c.Establecimientos)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdComerciante == id, cancellationToken);

        if (comerciante == null)
        {
            throw new NotFoundException($"No se encontró el comerciante con identificador {id}.");
        }

        return new ComercianteDto
        {
            IdComerciante = comerciante.IdComerciante,
            NombreRazonSocial = comerciante.NombreRazonSocial,
            IdMunicipio = comerciante.IdMunicipio,
            MunicipioNombre = comerciante.Municipio != null ? comerciante.Municipio.Nombre : string.Empty,
            Telefono = comerciante.Telefono,
            CorreoElectronico = comerciante.CorreoElectronico,
            FechaRegistro = comerciante.FechaRegistro,
            Estado = comerciante.Estado,
            FechaActualizacion = comerciante.FechaActualizacion,
            UsuarioActualizacion = comerciante.UsuarioActualizacion,
            CantidadEstablecimientos = comerciante.Establecimientos.Count,
            TotalIngresos = comerciante.Establecimientos.Sum(e => e.Ingresos),
            CantidadEmpleados = comerciante.Establecimientos.Sum(e => e.NumeroEmpleados)
        };
    }

    public async Task<ComercianteDto> CrearAsync(CrearComercianteDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await _crearValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            throw new ValidationAppException(validation.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var municipioExiste = await _context.Municipios.AnyAsync(m => m.IdMunicipio == dto.IdMunicipio, cancellationToken);
        if (!municipioExiste)
        {
            throw new ValidationAppException($"El municipio con Id {dto.IdMunicipio} no existe.");
        }

        var usuarioAuditoria = _currentUserService.UserName 
            ?? _currentUserService.Email 
            ?? "Sistema";

        var entidad = new Comerciante
        {
            NombreRazonSocial = dto.NombreRazonSocial.Trim(),
            IdMunicipio = dto.IdMunicipio,
            Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim(),
            CorreoElectronico = string.IsNullOrWhiteSpace(dto.CorreoElectronico) ? null : dto.CorreoElectronico.Trim(),
            FechaRegistro = dto.FechaRegistro.Date,
            Estado = dto.Estado,
            FechaActualizacion = DateTime.UtcNow,
            UsuarioActualizacion = usuarioAuditoria
        };

        _context.Comerciantes.Add(entidad);
        await _context.SaveChangesAsync(cancellationToken);

        return await ObtenerPorIdAsync(entidad.IdComerciante, cancellationToken);
    }

    public async Task<ComercianteDto> ActualizarAsync(int id, ActualizarComercianteDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await _actualizarValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            throw new ValidationAppException(validation.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var entidad = await _context.Comerciantes
            .Include(c => c.Establecimientos)
            .FirstOrDefaultAsync(c => c.IdComerciante == id, cancellationToken);

        if (entidad == null)
        {
            throw new NotFoundException($"No se encontró el comerciante con identificador {id}.");
        }

        var municipioExiste = await _context.Municipios.AnyAsync(m => m.IdMunicipio == dto.IdMunicipio, cancellationToken);
        if (!municipioExiste)
        {
            throw new ValidationAppException($"El municipio con Id {dto.IdMunicipio} no existe.");
        }

        var usuarioAuditoria = _currentUserService.UserName 
            ?? _currentUserService.Email 
            ?? "Sistema";

        entidad.NombreRazonSocial = dto.NombreRazonSocial.Trim();
        entidad.IdMunicipio = dto.IdMunicipio;
        entidad.Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim();
        entidad.CorreoElectronico = string.IsNullOrWhiteSpace(dto.CorreoElectronico) ? null : dto.CorreoElectronico.Trim();
        entidad.FechaRegistro = dto.FechaRegistro.Date;
        entidad.Estado = dto.Estado;
        entidad.FechaActualizacion = DateTime.UtcNow;
        entidad.UsuarioActualizacion = usuarioAuditoria;

        await _context.SaveChangesAsync(cancellationToken);

        return await ObtenerPorIdAsync(entidad.IdComerciante, cancellationToken);
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        var entidad = await _context.Comerciantes.FirstOrDefaultAsync(c => c.IdComerciante == id, cancellationToken);
        if (entidad == null)
        {
            throw new NotFoundException($"No se encontró el comerciante con identificador {id}.");
        }

        _context.Comerciantes.Remove(entidad);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ComercianteDto> ModificarEstadoAsync(int id, string nuevoEstado, CancellationToken cancellationToken = default)
    {
        var estadoDto = new ActualizarEstadoDto { Estado = nuevoEstado };
        var validation = await _estadoValidator.ValidateAsync(estadoDto, cancellationToken);
        if (!validation.IsValid)
        {
            throw new ValidationAppException(validation.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var entidad = await _context.Comerciantes.FirstOrDefaultAsync(c => c.IdComerciante == id, cancellationToken);
        if (entidad == null)
        {
            throw new NotFoundException($"No se encontró el comerciante con identificador {id}.");
        }

        var usuarioAuditoria = _currentUserService.UserName 
            ?? _currentUserService.Email 
            ?? "Sistema";

        entidad.Estado = nuevoEstado;
        entidad.FechaActualizacion = DateTime.UtcNow;
        entidad.UsuarioActualizacion = usuarioAuditoria;

        await _context.SaveChangesAsync(cancellationToken);

        return await ObtenerPorIdAsync(entidad.IdComerciante, cancellationToken);
    }
}

