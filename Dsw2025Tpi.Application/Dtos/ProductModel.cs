using System;

namespace Dsw2025Tpi.Application.Dtos
{
    
    public record ProductModel
    {
        public record Request(string Sku, string InternalCode, string Name, string? Description, decimal CurrentUnitPrice, int StockQuantity);
        public record Response(Guid Id, string Sku, string InternalCode, string Name, string? Description, decimal CurrentUnitPrice, int StockQuantity, bool IsActive);
        public record ResponsePagination(List<Response> ProductItems, int Total);
        public record FilterProduct(string? Status, string? Search, int? PageNumber, int? PageSize);

    }
}
