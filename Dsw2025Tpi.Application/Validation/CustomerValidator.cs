using System;
using System.Text.RegularExpressions;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;

namespace Dsw2025Tpi.Application.Validation
{
    public static class CustomerValidator
    {
        public static void Validate(CustomerModel.CustomerModelRequest request)
        {
            if (request == null)
                throw new BadRequestException("El cliente no puede ser nulo.");
            ValidateName(request.Name);
            ValidateEmail(request.Email);
            ValidatePhoneNumber(request.PhoneNumber);    
        }

        public static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BadRequestException("El email es obligatorio.");
            if(email.Length < 5 || email.Length > 254)
                throw new BadRequestException("El email debe tener entre 5 y 254 caracteres.");
            if (email.StartsWith('.') || email.EndsWith('.'))
                throw new BadRequestException("El email no puede comenzar o terminar con un punto '.'");
            var pattern = @"^[a-zA-Z0-9.!#$%&'+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)+$";
            var regex = new Regex(pattern);
            if (!regex.IsMatch(email))
                throw new BadRequestException($"Direccion de Email invalida: {email}");
        }

        public static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BadRequestException("El nombre es obligatorio.");
            if (name.Length < 3 || name.Length > 50)
                throw new BadRequestException("El nombre debe tener entre 3 y 50 caracteres.");
            if (name.StartsWith(' ') || name.EndsWith(' '))
                throw new BadRequestException("El nombre no puede comenzar o terminar con un espacio en blanco.");
        }

        public static void ValidatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new BadRequestException("El número de teléfono es obligatorio.");
            if (phoneNumber.Length < 7 || phoneNumber.Length > 15)
                throw new BadRequestException("El número de teléfono debe tener entre 7 y 15 caracteres.");
            if (!Regex.IsMatch(phoneNumber, @"^\d+$"))
                throw new BadRequestException("El número de teléfono solo puede contener dígitos.");
        }

    }
}
