using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationException = Dsw2025Tpi.Application.Exceptions.ApplicationException;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersManagementService _service;

        public OrdersController(OrdersManagementService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] OrderModel.OrderRequest request)
        {
            if (request == null || request.OrderItems == null || !request.OrderItems.Any())
                return BadRequest("Datos de la orden inválidos o incompletos.");

            try
            {
                var order = await _service.CreateOrderAsync(request);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:guid}")]
        public Task<IActionResult> GetOrderById(Guid id)
        {
            return Task.FromResult<IActionResult>(Ok());
        }
    }
}

