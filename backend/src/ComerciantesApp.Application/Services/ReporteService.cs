using System.Globalization;
using System.Text;
using ComerciantesApp.Application.DTOs.Reportes;
using ComerciantesApp.Application.Interfaces;

namespace ComerciantesApp.Application.Services;

public class ReporteService : IReporteService
{
    private readonly IApplicationDbContext _context;

    public ReporteService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ReporteComercianteDto>> ObtenerDatosReporteActivosAsync(CancellationToken cancellationToken = default)
    {
        // Consume la funcion de tabla en linea / procedimiento almacenado construido en el Reto 04
        var rawData = await _context.ObtenerReporteComerciantesActivosRawAsync(cancellationToken);

        return rawData.Select(r => new ReporteComercianteDto
        {
            NombreRazonSocial = r.NombreRazonSocial,
            Municipio = r.Municipio,
            Telefono = r.Telefono,
            CorreoElectronico = r.CorreoElectronico,
            FechaRegistro = r.FechaRegistro,
            Estado = r.Estado,
            CantidadEstablecimientos = r.CantidadEstablecimientos,
            TotalIngresos = r.TotalIngresos,
            CantidadEmpleados = r.CantidadEmpleados
        }).ToList();
    }

    public async Task<byte[]> GenerarReporteCsvPipeAsync(CancellationToken cancellationToken = default)
    {
        var datos = await ObtenerDatosReporteActivosAsync(cancellationToken);

        var sb = new StringBuilder();

        // Encabezado con separador pipe (|)
        sb.AppendLine("Nombre o razon social|Municipio|Telefono|Correo Electronico|Fecha de Registro|Estado|Cantidad de Establecimientos|Total Ingresos|Cantidad de Empleados");

        var numberFormat = new NumberFormatInfo
        {
            NumberDecimalSeparator = ".",
            NumberGroupSeparator = ""
        };

        foreach (var item in datos)
        {
            var linea = string.Join("|", new string[]
            {
                item.NombreRazonSocial.Replace("|", " "),
                item.Municipio.Replace("|", " "),
                (item.Telefono ?? string.Empty).Replace("|", " "),
                (item.CorreoElectronico ?? string.Empty).Replace("|", " "),
                item.FechaRegistro.ToString("yyyy-MM-dd"),
                item.Estado,
                item.CantidadEstablecimientos.ToString(),
                item.TotalIngresos.ToString("F2", numberFormat),
                item.CantidadEmpleados.ToString()
            });

            sb.AppendLine(linea);
        }

        // Retornar en codificacion UTF-8 con BOM para correcta apertura en Excel y editores
        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
    }
}

