using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            if (request == null || request.OrderItems == null || request.OrderItems.Count == 0)
                return BadRequest("Datos de la orden inválidos o incompletos.");

            try
            {
                var order = await _service.CreateOrderAsync(request);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
            }
            catch (InvalidOperationException ex)
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
        public async Task<IActionResult> GetAllOrders([FromQuery]OrderModel.OrderRequestFilter filter)
        {
            try
            {
                var result = await _service.GetAllOrdersAsync(filter);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return StatusCode(500, "Fallo inesperaco del servidor");
            }
            
        }




        [HttpGet("{id:guid}")]
        public Task<IActionResult> GetOrderById(Guid id)
        {
            return Task.FromResult<IActionResult>(Ok());
        }
    }
}

