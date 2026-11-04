namespace ComerciantesApp.Domain.Entities;

public class ReporteComercianteActivo
{
    public string NombreRazonSocial { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int CantidadEstablecimientos { get; set; }
    public decimal TotalIngresos { get; set; }
    public int CantidadEmpleados { get; set; }
}

