using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationException = Dsw2025Tpi.Application.Exceptions.ApplicationException;


namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductsManagementService _service;

    public ProductsController(ProductsManagementService service)
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

    [HttpGet("{id:guid}", Name = "GetProductById")]
    public async Task<IActionResult> GetProductByIdAsync(Guid id)
    {
        var product = await _service.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpPost()]
    public async Task<IActionResult> CreateProductAsync([FromBody] ProductModel.Request request)
    {
        if (request == null)
        {
            return BadRequest("Product data is required.");
        }
        var createdProduct = await _service.AddProduct(request);
        //return CreatedAtAction(nameof(GetProductByIdAsync), new { id = createdProduct.Id }, createdProduct);
        return CreatedAtRoute("GetProductById", new { id = createdProduct.Id }, createdProduct);
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateProductAsync(Guid id, [FromBody] ProductModel.Request request)
    {
        if (request == null)
        {
            return BadRequest("Product data is invalid.");
        }
        var updatedProduct = await _service.UpdateProduct(id, request);
        if (updatedProduct == null)
        {
            return NotFound();
        }
        return Ok(updatedProduct);
    }

  /*  [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> DeleteProductAsync(Guid id)
    {
        var deleted = await _service.DeactivateProduct(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }*/

    [HttpPatch()]
    [Route("{id:guid}")]
    public async Task<IActionResult> PatchProduct(Guid id)
    {
        try
        {
            var product = await _service.DeactivateProduct(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (ApplicationException de)
        {
            return Conflict(de.Message);
        }
        catch (Exception)
        {
            return Problem("Se produjo un error al actualizar el producto");
        }
    }
}