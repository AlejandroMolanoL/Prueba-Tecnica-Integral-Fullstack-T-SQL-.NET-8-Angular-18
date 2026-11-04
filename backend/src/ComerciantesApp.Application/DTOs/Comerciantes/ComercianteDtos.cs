namespace ComerciantesApp.Application.DTOs.Comerciantes;

public class ComercianteDto
{
    public int IdComerciante { get; set; }
    public string NombreRazonSocial { get; set; } = string.Empty;
    public int IdMunicipio { get; set; }
    public string MunicipioNombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? CorreoElectronico { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string Estado { get; set; } = "Activo";
    public DateTime? FechaActualizacion { get; set; }
    public string? UsuarioActualizacion { get; set; }

    // Resumen de establecimientos asociados (Reto 04, Reto 10 y Reto 11)
    public int CantidadEstablecimientos { get; set; }
    public decimal TotalIngresos { get; set; }
    public int CantidadEmpleados { get; set; }
}

public class CrearComercianteDto
{
    public string NombreRazonSocial { get; set; } = string.Empty;
    public int IdMunicipio { get; set; }
    public string? Telefono { get; set; }
    public string? CorreoElectronico { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string Estado { get; set; } = "Activo";
}

public class ActualizarComercianteDto
{
    public string NombreRazonSocial { get; set; } = string.Empty;
    public int IdMunicipio { get; set; }
    public string? Telefono { get; set; }
    public string? CorreoElectronico { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string Estado { get; set; } = "Activo";
}

public class ActualizarEstadoDto
{
    public string Estado { get; set; } = string.Empty;
}

public class ComercianteFiltrosDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public string? NombreRazonSocial { get; set; }
    public DateTime? FechaRegistro { get; set; }
    public string? Estado { get; set; }
}

