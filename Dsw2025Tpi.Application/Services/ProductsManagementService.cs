using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Services;

public class ProductsManagementService
{
    private readonly IRepository _repository;

    public ProductsManagementService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductModel.Response?> GetProductById(Guid id)
    {
        var product= await _repository.GetById<Product>(id);
        if (product == null)
            return null;

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
        if (string.IsNullOrWhiteSpace(request.Sku) ||
            string.IsNullOrWhiteSpace(request.InternalCode) ||
            string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Description) ||
            string.IsNullOrWhiteSpace(request.CurrentUnitPrice.ToString()) ||
            string.IsNullOrWhiteSpace(request.StockQuantity.ToString()))
        {
            throw new ArgumentException("Invalid values for product");
        }

        var exist = await _repository.First<Product>(p => p.Sku == request.Sku);
        if (exist != null) throw new DuplicatedEntityException($"A product with that Sku already exists {request.Sku}");
        if (exist != null) throw new DuplicatedEntityException($"A product with that Internal Code already exists {request.InternalCode}");

        var product = new Product(request.Sku, request.InternalCode, request.Name, request.Description, request.CurrentUnitPrice, request.StockQuantity);
        await _repository.Add(product);
        return new ProductModel.Response(product.Id, product.Sku, product.InternalCode,product.Name, product.Description, product.CurrentUnitPrice, product.StockQuantity, product.IsActive);
    }

    public async Task<ProductModel.Response> UpdateProduct(Guid id, ProductModel.Request request)
    {
        var product = await _repository.GetById<Product>(id);
        if (product == null)
            throw new System.ApplicationException("Producto no encontrado.");
        if (string.IsNullOrWhiteSpace(request.Sku) || string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("SKU y nombre son obligatorios.");
        product.Sku = request.Sku;
        product.InternalCode = request.InternalCode;
        product.Name = request.Name;
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

    public async Task<ProductModel.Response?> DeactivateProduct(Guid id)
    {
        var product = await _repository.GetById<Product>(id);
        if (product == null)
            return null;

        product.IsActive = false;
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
}

