namespace ComerciantesApp.Domain.Entities;

public class Comerciante
{
    public int IdComerciante { get; set; }
    public string NombreRazonSocial { get; set; } = string.Empty;
    public int IdMunicipio { get; set; }
    public Municipio Municipio { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? CorreoElectronico { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow.Date;
    public string Estado { get; set; } = "Activo";

    // Campos de auditoria obligatorios (Reto 01 & Reto 02)
    public DateTime? FechaActualizacion { get; set; }
    public string? UsuarioActualizacion { get; set; }

    public ICollection<Establecimiento> Establecimientos { get; set; } = new List<Establecimiento>();
}
