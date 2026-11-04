namespace ComerciantesApp.Domain.Entities;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string ContrasenaHash { get; set; } = string.Empty;
    public int IdRol { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public Rol Rol { get; set; } = null!;
}

