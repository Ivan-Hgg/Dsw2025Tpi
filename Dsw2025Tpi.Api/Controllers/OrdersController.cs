using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using ApplicationException = Dsw2025Tpi.Application.Exceptions.ApplicationException;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize(Roles = "Cliente")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersManagementService _service;

        public OrdersController(IOrdersManagementService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] OrderModel.OrderRequest request)
        {
            try
            {
                var order = await _service.CreateOrderAsync(request);
                return StatusCode(201, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (EntityNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllOrders([FromQuery] OrderModel.OrderRequestFilter filter)
        {
            try
            {
                var result = await _service.GetAllOrdersAsync(filter);
                return Ok(result);
            }catch(EntityNotFoundException ex) { 
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Fallo inesperado del servidor");
            }

        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            try
            {
                var result = await _service.GetOrderByIdAsync(id);
                return Ok(result);

            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

        }


        [HttpPatch("{id:guid}/status")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] OrderModel.OrderRequestStatus status)
        {
            try
            {
                var result = await _service.UpdateOrderStatusAsync(id, status);
                return Ok(result);
            }
            catch (ArgumentException ex){
                return BadRequest(ex.Message);
            }
            catch(EntityNotFoundException ex){
                return NotFound(ex.Message);
            }
        }
    }
}

