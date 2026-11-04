using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ComerciantesApp.Application.Interfaces;
using ComerciantesApp.Application.Services;

namespace ComerciantesApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMunicipioService, MunicipioService>();
        services.AddScoped<IComercianteService, ComercianteService>();
        services.AddScoped<IReporteService, ReporteService>();

        return services;
    }
}
