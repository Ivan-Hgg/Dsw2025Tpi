using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Exceptions;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Authorization;


namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles ="ADMINISTRADOR")]
public class ProductsController : ControllerBase
{
    private readonly IProductsManagementService _service;

    public ProductsController(IProductsManagementService service)
    {
        _service = service;
    }

    [HttpGet()]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllProductsAsync()
    {
        var products = await _service.GetAllProducts();
        if(products is null || !products.Any()) return NoContent();
        return Ok(products);        
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductByIdAsync(Guid id)
    {
        var result = await _service.GetProductById(id);
        return Ok(result);
    }

    [HttpPost()]
    public async Task<IActionResult> CreateProductAsync([FromBody] ProductModel.Request request)
    {
        var result = await _service.AddProduct(request);
        return StatusCode(201, result);
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateProdcut(Guid id,[FromBody] ProductModel.Request request)
    {
        var result = await _service.UpdateProduct(id, request);
        return Ok(result);
    }

    [HttpPatch()]
    [Route("{id:guid}")]
    public async Task<IActionResult> DeactivateProduct(Guid id)
    {
        await _service.DeactivateProduct(id);
        return NoContent();
    }
}
