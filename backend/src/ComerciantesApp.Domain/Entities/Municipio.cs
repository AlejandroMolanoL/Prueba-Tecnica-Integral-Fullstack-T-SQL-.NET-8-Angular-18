namespace ComerciantesApp.Domain.Entities;

public class Municipio
{
    public int IdMunicipio { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string? CodigoDANE { get; set; }

    public ICollection<Comerciante> Comerciantes { get; set; } = new List<Comerciante>();
}

