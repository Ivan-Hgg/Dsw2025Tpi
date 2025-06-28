using System;

namespace Dsw2025Tpi.Application.Dtos
{
    
    public record ProductModel
    {
        public record Request(string Sku, string InternalCode, string Name, string? Description, decimal CurrentUnitPrice, int StockQuantity);
        public record Response(Guid Id, string Sku, string InternalCode, string Name, string? Description, decimal CurrentUnitPrice, int StockQuantity, bool IsActive);
    }
}
