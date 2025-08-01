using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Exceptions;
using Microsoft.AspNetCore.Mvc.TagHelpers;


namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductsManagementService _service;

    public ProductsController(IProductsManagementService service)
    {
        _service = service;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAllProductsAsync()
    {
        var products = await _service.GetAllProducts();
        if (products == null || !products.Any()) return NoContent();
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductByIdAsync(Guid id)
    {
        try
        {
            var result = await _service.GetProductById(id);
            return Ok(result);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost()]
    public async Task<IActionResult> CreateProductAsync([FromBody] ProductModel.Request request)
    {
        try
        {
            var result = await _service.AddProduct(request);
            return StatusCode(201, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (DuplicatedEntityException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateProdcut(Guid id,[FromBody] ProductModel.Request request)
    {
        try
        {
            var result = await _service.UpdateProduct(id, request);
            return Ok(result);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch()]
    [Route("{id:guid}")]
    public async Task<IActionResult> DeactivateProduct(Guid id)
    {
        try
        {
            await _service.DeactivateProduct(id);
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
