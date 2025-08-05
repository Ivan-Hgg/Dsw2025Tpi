using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using System;
using System.Text.RegularExpressions;

namespace Dsw2025Tpi.Application.Validation
{
    public static class ProductValidator
    {
        public static void Validate(ProductModel.Request request)
        {
            if (request == null)
                throw new BadRequestException("El producto no puede ser nulo.");
            ValidateSku(request.Sku);
            ValidateInternalCode(request.InternalCode);
            ValidateName(request.Name);
            ValidateDescription(request.Description);

            if (request.CurrentUnitPrice <= 0)
                throw new BadRequestException("El precio debe ser mayor a cero.");

            if (request.StockQuantity < 0)
                throw new BadRequestException("El stock debe ser un valor positivo.");
        }

        public static void ValidateSku(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new BadRequestException("El SKU es obligatorio.");
            var pattern = @"^[A-Z0-9_-]{3,10}$";
            if (!Regex.IsMatch(sku,pattern))
                throw new BadRequestException("El SKU debe contener entre 3 y 10 caracteres alfanuméricos, guiones bajos o guiones medios.");
        }
        public static void ValidateInternalCode(string internalCode)
        {
            if (string.IsNullOrWhiteSpace(internalCode))
                throw new BadRequestException("El código interno es obligatorio.");
            var pattern = @"^INT-[0-9]{3,10}$";
            if (!Regex.IsMatch(internalCode, pattern))
                throw new BadRequestException("El codigo interno debe comenzar con 'INT-' seguido de 3 a 10 digitos");
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

        public static void ValidateDescription(string? description)
        {
            if (description != null && description.Length > 200)
                throw new BadRequestException("La descripción no puede exceder los 200 caracteres.");
        }

    }

}