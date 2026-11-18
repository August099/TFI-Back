using Application.Interfaces;
using Application.Models.Order;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<OrderDTO>>> GetAllAsync()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetByIdAsync([FromRoute] int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) 
                return NotFound($"La orden con el Id:{id} no pudo ser encontrada");

            return Ok(order);
        }

        [HttpGet("client/{clientId}")]
        [Authorize(Roles = "Admin,Recepcionista,Cliente")]
        public async Task<ActionResult<List<OrderDTO>>> GetByClientIdAsync([FromRoute] int clientId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // para que no traiga otras ordenes de otro cliente
            if (userRole == "Cliente")
            {
                if (userIdClaim == null || int.Parse(userIdClaim) != clientId)
                {
                    return Forbid(); // error 403
                }
            }

            var orders = await _orderService.GetByClientIdAsync(clientId);
            return Ok(orders);
        }

        [HttpGet("status/{state}")]
        [Authorize(Roles = "Admin,Recepcionista")]
        public async Task<ActionResult<List<OrderDTO>>> GetByStateAsync([FromRoute] OrderStatus state)
        {
            var orders = await _orderService.GetByStateAsync(state);
            return Ok(orders);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Recepcionista,Cliente")]
        public async Task<ActionResult<OrderDTO>> Add([FromBody] CreateOrderDTO order)
        {
            var createdOrder = await _orderService.AddAsync(order);
            if (createdOrder == null) 
                return BadRequest($"La orden no pudo ser creada");

            return CreatedAtAction(
                actionName: nameof(GetByIdAsync),
                routeValues: new { id = createdOrder.Id },
                value: createdOrder
            );
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Recepcionista")]
        public async Task<ActionResult<OrderDTO>> Update([FromBody] OrderDTO order, [FromRoute] int id)
        {
            if (order == null) 
                return BadRequest("Los datos de la orden están incompletos");
            if (order.Id != id) 
                return BadRequest("El Id de la ruta no coincide con el de la orden");

            var updateOrder = await _orderService.UpdateAsync(order);
            if (updateOrder == null) 
                return NotFound($"No se encontró la orden con el Id: {order.Id}");

            return Ok(updateOrder);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Recepcionista")]
        public async Task<ActionResult<OrderDTO>> Delete([FromRoute] int id)
        {
            var order = await _orderService.DeleteAsync(id);
            if (order == null) 
                return NotFound($"La orden con el Id:{id} no pudo ser eliminada");

            return Ok(order);
        }
    }
}