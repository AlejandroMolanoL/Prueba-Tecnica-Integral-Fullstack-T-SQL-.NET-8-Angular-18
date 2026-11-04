# Imagen base para compilacion con SDK de .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar archivos de proyecto y restaurar dependencias
COPY ComerciantesApp.sln ./
COPY src/ComerciantesApp.Domain/ComerciantesApp.Domain.csproj src/ComerciantesApp.Domain/
COPY src/ComerciantesApp.Application/ComerciantesApp.Application.csproj src/ComerciantesApp.Application/
COPY src/ComerciantesApp.Infrastructure/ComerciantesApp.Infrastructure.csproj src/ComerciantesApp.Infrastructure/
COPY src/ComerciantesApp.WebApi/ComerciantesApp.WebApi.csproj src/ComerciantesApp.WebApi/
COPY tests/ComerciantesApp.Tests/ComerciantesApp.Tests.csproj tests/ComerciantesApp.Tests/

RUN dotnet restore

# Copiar el codigo fuente completo y compilar
COPY . ./
WORKDIR /app/src/ComerciantesApp.WebApi
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Imagen runtime optimizada de ASP.NET Core 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ComerciantesApp.WebApi.dll"]

