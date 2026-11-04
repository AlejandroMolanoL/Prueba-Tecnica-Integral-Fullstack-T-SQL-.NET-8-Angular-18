using FluentValidation;
using ComerciantesApp.Application.DTOs.Auth;
using ComerciantesApp.Application.DTOs.Comerciantes;

namespace ComerciantesApp.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.CorreoElectronico)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.")
            .MaximumLength(150).WithMessage("El correo electrónico no puede exceder 150 caracteres.");

        RuleFor(x => x.Contrasena)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(6).WithMessage("La contraseña debe tener mínimo 6 caracteres.");
    }
}

public class CrearComercianteValidator : AbstractValidator<CrearComercianteDto>
{
    public CrearComercianteValidator()
    {
        RuleFor(x => x.NombreRazonSocial)
            .NotEmpty().WithMessage("El Nombre o Razón Social es obligatorio.")
            .MaximumLength(150).WithMessage("El Nombre o Razón Social no puede exceder 150 caracteres.");

        RuleFor(x => x.IdMunicipio)
            .GreaterThan(0).WithMessage("Debe seleccionar un municipio válido.");

        RuleFor(x => x.Telefono)
            .MaximumLength(30).WithMessage("El teléfono no puede exceder 30 caracteres.")
            .Matches(@"^[+0-9\s\-()]*$").WithMessage("El teléfono contiene caracteres no válidos.")
            .When(x => !string.IsNullOrWhiteSpace(x.Telefono));

        RuleFor(x => x.CorreoElectronico)
            .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.")
            .MaximumLength(150).WithMessage("El correo electrónico no puede exceder 150 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.CorreoElectronico));

        RuleFor(x => x.FechaRegistro)
            .NotEmpty().WithMessage("La fecha de registro es obligatoria.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1)).WithMessage("La fecha de registro no puede ser futura.");

        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("El estado es obligatorio.")
            .Must(e => e == "Activo" || e == "Inactivo")
            .WithMessage("El estado debe ser 'Activo' o 'Inactivo'.");
    }
}

public class ActualizarComercianteValidator : AbstractValidator<ActualizarComercianteDto>
{
    public ActualizarComercianteValidator()
    {
        RuleFor(x => x.NombreRazonSocial)
            .NotEmpty().WithMessage("El Nombre o Razón Social es obligatorio.")
            .MaximumLength(150).WithMessage("El Nombre o Razón Social no puede exceder 150 caracteres.");

        RuleFor(x => x.IdMunicipio)
            .GreaterThan(0).WithMessage("Debe seleccionar un municipio válido.");

        RuleFor(x => x.Telefono)
            .MaximumLength(30).WithMessage("El teléfono no puede exceder 30 caracteres.")
            .Matches(@"^[+0-9\s\-()]*$").WithMessage("El teléfono contiene caracteres no válidos.")
            .When(x => !string.IsNullOrWhiteSpace(x.Telefono));

        RuleFor(x => x.CorreoElectronico)
            .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.")
            .MaximumLength(150).WithMessage("El correo electrónico no puede exceder 150 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.CorreoElectronico));

        RuleFor(x => x.FechaRegistro)
            .NotEmpty().WithMessage("La fecha de registro es obligatoria.");

        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("El estado es obligatorio.")
            .Must(e => e == "Activo" || e == "Inactivo")
            .WithMessage("El estado debe ser 'Activo' o 'Inactivo'.");
    }
}

public class ActualizarEstadoValidator : AbstractValidator<ActualizarEstadoDto>
{
    public ActualizarEstadoValidator()
    {
        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("El estado es obligatorio.")
            .Must(e => e == "Activo" || e == "Inactivo")
            .WithMessage("El estado debe ser 'Activo' o 'Inactivo'.");
    }
}

