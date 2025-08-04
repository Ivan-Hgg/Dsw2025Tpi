using Dsw2025Tpi.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Validation;

public static class AuthenticateValidator
{
    public static void ValidateRegisterModel(RegisterModelRequest model)
    {
        if (model == null)
            throw new InvalidOperationException("El modelo de registro no puede ser nulo.");
        ValidateUsername(model.Username);
        CustomerValidator.Validate(model.Customer);
        ValidatePassword(model.Password);
        ValidateRole(model.Role);
    }

    public static void ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new InvalidOperationException("El nombre de usuario es obligatorio.");
        if (username.Length < 3 || username.Length > 50)
            throw new InvalidOperationException("El nombre de usuario debe tener entre 3 y 50 caracteres.");
        if (username.StartsWith(' ') || username.EndsWith(' '))
            throw new InvalidOperationException("El nombre de usuario no puede comenzar o terminar con un espacio en blanco.");
        var pattern = @"^[a-zA-Z0-9._-]+$";
        if(!Regex.IsMatch(username, pattern))
            throw new InvalidOperationException($"El nombre de usuario es invalido: {username}" +
                "\nsolo puede contener letras, números, puntos, guiones bajos y guiones.");
    }

    public static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("La contraseña es obligatoria.");
    }

    public static void ValidateRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new InvalidOperationException("El rol es obligatorio.");
        if (role.ToUpper() != "ADMINISTRADOR" && role.ToUpper() != "CLIENTE")
            throw new InvalidOperationException($"El rol ingresado es desconocido: {role} "+" Posibles roles: 'Administrador' o 'Cliente'.");
    }


}
