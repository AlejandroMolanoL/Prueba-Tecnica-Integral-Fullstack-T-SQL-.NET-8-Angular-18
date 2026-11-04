namespace ComerciantesApp.Application.DTOs.Municipios;

public class MunicipioDto
{
    public int IdMunicipio { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string? CodigoDANE { get; set; }
    public string NombreCompleto => string.IsNullOrEmpty(Departamento) 
        ? Nombre 
        : $"{Nombre} ({Departamento})";
}
