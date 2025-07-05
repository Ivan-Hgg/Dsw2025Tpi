using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Validation;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Services;

public class ProductsManagementService : IProductsManagementService
{
    private readonly IRepository _repository;

    public ProductsManagementService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductModel.Response?> GetProductById(Guid id)
    {
        var product = await _repository.GetById<Product>(id);
        if (product == null)
            throw new EntityNotFoundException("Producto no encontrado");

        return new ProductModel.Response(
            product.Id,
            product.Sku,
            product.InternalCode,
            product.Name,
            product.Description,
            product.CurrentUnitPrice,
            product.StockQuantity,
            product.IsActive
        );
    }

    public async Task<IEnumerable<ProductModel.Response>?> GetAllProducts()
    {
        var products = await _repository.GetFiltered<Product>(p => p.IsActive);
        return products?.Select(p => new ProductModel.Response(
            p.Id,
            p.Sku,
            p.InternalCode,
            p.Name,
            p.Description,
            p.CurrentUnitPrice,
            p.StockQuantity,
            p.IsActive
        ));
    }

    public async Task<ProductModel.Response> AddProduct(ProductModel.Request request)
    {
        ProductValidator.Validate(request);

        var existSku = await _repository.First<Product>(p => p.Sku == request.Sku);
        var existInternalCode = await _repository.First<Product>(p => p.InternalCode == request.InternalCode);

        if (existSku != null)
        {
            throw new DuplicatedEntityException($"Un producto con el mismo Sku ya existe {request.Sku}");
        }
        if (existInternalCode != null)
        {
            throw new DuplicatedEntityException($"Un producto con el mismo InternalCode ya existe {request.InternalCode}");
        }

        var description = request.Description ?? string.Empty;

        var product = new Product(request.Sku, request.InternalCode, request.Name, description, request.CurrentUnitPrice, request.StockQuantity);
        await _repository.Add(product);
        return new ProductModel.Response(product.Id, product.Sku, product.InternalCode, product.Name, product.Description, product.CurrentUnitPrice, product.StockQuantity, product.IsActive);
    }

    public async Task<ProductModel.Response> UpdateProduct(Guid id, ProductModel.Request request)
    { 
            ProductValidator.Validate(request);

        var product = await _repository.GetById<Product>(id) ?? throw new System.ApplicationException("Producto no encontrado.");

        

        product.Sku = request.Sku;
        product.InternalCode = request.InternalCode;
        product.Name = request.Name;
        product.Description = request.Description;
        product.CurrentUnitPrice = request.CurrentUnitPrice;
        product.StockQuantity = request.StockQuantity;

        var updated = await _repository.Update(product);
        return new ProductModel.Response(
            updated.Id,
            updated.Sku,
            updated.InternalCode,
            updated.Name,
            updated.Description,
            updated.CurrentUnitPrice,
            updated.StockQuantity,
            updated.IsActive
        );
    }

    public async Task DeactivateProduct(Guid id)
    {
        var product = await _repository.GetById<Product>(id);
        if (product == null)
            throw new EntityNotFoundException("Producto no encontrado.");

        product.IsActive = false;
        await _repository.Update(product);
    }
}
