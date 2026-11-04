using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ComerciantesApp.Application;
using ComerciantesApp.Application.Interfaces;
using ComerciantesApp.Infrastructure;
using ComerciantesApp.Infrastructure.Data;
using ComerciantesApp.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. Capas Clean Architecture
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// 2. Controladores con serializacion JSON estandar
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// 3. Politica de CORS (Reto 05)
const string CorsPolicyName = "AllowFrontendClient";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200", 
                "https://localhost:4200",
                "http://127.0.0.1:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 4. Swagger / OpenAPI con soporte para JWT Bearer (Reto 04 / Reglas .NET)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Comerciantes y Establecimientos - OL Software",
        Version = "v1",
        Description = "API REST construida en .NET 8 con Clean Architecture, Entity Framework Core y seguridad JWT para la agremiación nacional de comercio."
    });

    // Definicion de seguridad JWT en Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT obtenido del endpoint de login. Ejemplo: Bearer {su_token_jwt}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 5. Pipeline HTTP y Middleware de Manejo Global de Excepciones
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Habilitar Swagger siempre (util para revision y evaluacion)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Comerciantes v1");
    c.RoutePrefix = string.Empty; // Carga Swagger en la raiz http://localhost:5000/
});

app.UseCors(CorsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 6. Inicializacion y datos semilla automaticos (Reto 03)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher>();

        // Si se usa SQL Server y la base de datos no existe, se asegura su creacion
        if (context.Database.IsRelational())
        {
            await context.Database.EnsureCreatedAsync();
        }

        await DbInitializer.SeedAsync(context, passwordHasher);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error durante la inicializacion o siembra de datos de la base de datos.");
    }
}

app.Run();

// Clase parcial publica para soportar pruebas de integracion con WebApplicationFactory
public partial class Program { }
