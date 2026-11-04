namespace ComerciantesApp.Domain.Entities;

public class Establecimiento
{
    public int IdEstablecimiento { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Ingresos { get; set; }
    public int NumeroEmpleados { get; set; }
    public int IdComerciante { get; set; }
    public Comerciante Comerciante { get; set; } = null!;

    // Campos de auditoria obligatorios (Reto 01 & Reto 02)
    public DateTime? FechaActualizacion { get; set; }
    public string? UsuarioActualizacion { get; set; }
}
