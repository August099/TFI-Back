using Application.Interfaces;
using Application.Models.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<CartDTO>> Get([FromRoute] int id)
        {
            var cart = await _cartService.GetByIdAsync(id);
            if (cart == null)
                return NotFound($"El Carro con el Id:{id} no pudo ser encontrado");

            var isStaff = User.IsInRole("Admin") || User.IsInRole("Recepcionista");
            if (!isStaff)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized();

                if (cart.ClientId != userId)
                    return Forbid();
            }

            return Ok(cart);
        }

        [HttpGet()]
        [Authorize(Roles = "Cliente,Veterinario")]
        public async Task<ActionResult<CartDTO>> GetMyCart()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var cart = await _cartService.GetOrCreateByClientIdAsync(userId);
            if (cart == null)
                return NotFound($"No se pudo crear el Carro");

            return Ok(cart);
        }
    }
}
