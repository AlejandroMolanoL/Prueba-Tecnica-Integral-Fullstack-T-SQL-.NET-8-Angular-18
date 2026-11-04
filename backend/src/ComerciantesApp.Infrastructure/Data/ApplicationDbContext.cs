using Microsoft.EntityFrameworkCore;
using ComerciantesApp.Application.Interfaces;
using ComerciantesApp.Domain.Entities;

namespace ComerciantesApp.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Municipio> Municipios => Set<Municipio>();
    public DbSet<Comerciante> Comerciantes => Set<Comerciante>();
    public DbSet<Establecimiento> Establecimientos => Set<Establecimiento>();
    public DbSet<ReporteComercianteActivo> ReportesComerciantesActivos => Set<ReporteComercianteActivo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Rol
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(r => r.IdRol);
            entity.Property(r => r.IdRol).ValueGeneratedOnAdd();
            entity.Property(r => r.Nombre).HasMaxLength(50).IsRequired();
            entity.Property(r => r.Descripcion).HasMaxLength(200);
            entity.HasIndex(r => r.Nombre).IsUnique();
        });

        // Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(u => u.IdUsuario);
            entity.Property(u => u.IdUsuario).ValueGeneratedOnAdd();
            entity.Property(u => u.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(u => u.CorreoElectronico).HasMaxLength(150).IsRequired();
            entity.Property(u => u.ContrasenaHash).HasMaxLength(255).IsRequired();
            entity.HasIndex(u => u.CorreoElectronico).IsUnique();

            entity.HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.IdRol)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Municipio
        modelBuilder.Entity<Municipio>(entity =>
        {
            entity.ToTable("Municipios");
            entity.HasKey(m => m.IdMunicipio);
            entity.Property(m => m.IdMunicipio).ValueGeneratedOnAdd();
            entity.Property(m => m.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(m => m.Departamento).HasMaxLength(100).IsRequired();
            entity.Property(m => m.CodigoDANE).HasMaxLength(10);
        });

        // Comerciante
        modelBuilder.Entity<Comerciante>(entity =>
        {
            entity.ToTable("Comerciantes");
            entity.HasKey(c => c.IdComerciante);
            entity.Property(c => c.IdComerciante).ValueGeneratedOnAdd();
            entity.Property(c => c.NombreRazonSocial).HasMaxLength(150).IsRequired();
            entity.Property(c => c.Telefono).HasMaxLength(30);
            entity.Property(c => c.CorreoElectronico).HasMaxLength(150);
            entity.Property(c => c.FechaRegistro).HasColumnType("date").IsRequired();
            entity.Property(c => c.Estado).HasMaxLength(10).HasDefaultValue("Activo").IsRequired();

            // Auditoria
            entity.Property(c => c.FechaActualizacion).HasColumnType("datetime2");
            entity.Property(c => c.UsuarioActualizacion).HasMaxLength(100);

            entity.HasOne(c => c.Municipio)
                .WithMany(m => m.Comerciantes)
                .HasForeignKey(c => c.IdMunicipio)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Establecimiento
        modelBuilder.Entity<Establecimiento>(entity =>
        {
            entity.ToTable("Establecimientos");
            entity.HasKey(e => e.IdEstablecimiento);
            entity.Property(e => e.IdEstablecimiento).ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Ingresos).HasColumnType("decimal(18,2)").HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.NumeroEmpleados).IsRequired();

            // Auditoria
            entity.Property(e => e.FechaActualizacion).HasColumnType("datetime2");
            entity.Property(e => e.UsuarioActualizacion).HasMaxLength(100);

            entity.HasOne(e => e.Comerciante)
                .WithMany(c => c.Establecimientos)
                .HasForeignKey(e => e.IdComerciante)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Keyless entity para Reto 04 / Reto 08: Reporte de comerciantes activos
        modelBuilder.Entity<ReporteComercianteActivo>(entity =>
        {
            entity.HasNoKey();
            entity.ToView(null);
            entity.Property(r => r.TotalIngresos).HasPrecision(18, 2);
        });
    }

    public async Task<IReadOnlyList<ReporteComercianteActivo>> ObtenerReporteComerciantesActivosRawAsync(CancellationToken cancellationToken = default)
    {
        // Si el proveedor de base de datos es SQL Server, ejecutamos la funcion de base de datos creada en el Reto 04
        if (Database.IsSqlServer())
        {
            return await ReportesComerciantesActivos
                .FromSqlRaw("SELECT * FROM dbo.fn_ObtenerReporteComerciantesActivos()")
                .ToListAsync(cancellationToken);
        }

        // Proveedor en memoria o fallback para pruebas unitarias xUnit
        return await Comerciantes
            .Include(c => c.Municipio)
            .Include(c => c.Establecimientos)
            .Where(c => c.Estado == "Activo")
            .Select(c => new ReporteComercianteActivo
            {
                NombreRazonSocial = c.NombreRazonSocial,
                Municipio = c.Municipio != null ? c.Municipio.Nombre : string.Empty,
                Telefono = c.Telefono ?? string.Empty,
                CorreoElectronico = c.CorreoElectronico ?? string.Empty,
                FechaRegistro = c.FechaRegistro,
                Estado = c.Estado,
                CantidadEstablecimientos = c.Establecimientos.Count,
                TotalIngresos = c.Establecimientos.Sum(e => e.Ingresos),
                CantidadEmpleados = c.Establecimientos.Sum(e => e.NumeroEmpleados)
            })
            .OrderByDescending(r => r.CantidadEstablecimientos)
            .ThenByDescending(r => r.TotalIngresos)
            .ToListAsync(cancellationToken);
    }
}
