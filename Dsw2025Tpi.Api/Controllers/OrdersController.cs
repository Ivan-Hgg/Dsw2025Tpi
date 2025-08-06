using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using ApplicationException = Dsw2025Tpi.Application.Exceptions.ApplicationException;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersManagementService _service;

        public OrdersController(IOrdersManagementService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> CreateOrderAsync([FromBody] OrderModel.OrderRequest request)
        {
            var order = await _service.CreateOrderAsync(request);
            return StatusCode(201, order);
            
        }

        [HttpGet]
        [Authorize(Roles ="ADMINISTRADOR, CLIENTE")]
        public async Task<IActionResult> GetAllOrders([FromQuery] OrderModel.OrderRequestFilter filter)
        {
            var result = await _service.GetAllOrdersAsync(filter);
            if (result is null || !result.Any()) return NoContent();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "ADMINISTRADOR, CLIENTE")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var result = await _service.GetOrderByIdAsync(id);
            return Ok(result);
        }


        [HttpPatch("{id:guid}/status")]
        [Authorize(Roles ="ADMINISTRADOR")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] OrderModel.OrderRequestStatus status)
        {
            var result = await _service.UpdateOrderStatusAsync(id, status);
            return Ok(result);
        }
    }
}

