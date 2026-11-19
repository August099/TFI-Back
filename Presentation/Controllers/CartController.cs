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

        [HttpGet("{id}")]
        public async Task<ActionResult<CartDTO>> Get([FromRoute] int id)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = await _cartService.GetByIdAsync(id);
            if (cart == null)
                return NotFound($"El Carro con el Id:{id} no pudo ser encontrado");

            // verificar que el carro pertenezca al usuario
            if (userRole == "Cliente" || userRole == "Veterinario")
            {
                if (string.IsNullOrEmpty(userIdClaim))
                    return BadRequest("No se pudo identificar al usuario");



                if (cart.ClientId != int.Parse(userIdClaim))
                    return Forbid();
            }
            return Ok(cart);
        }

        [HttpGet("cart")]
        [Authorize(Roles = "Cliente,Veterinario")]
        public async Task<ActionResult<CartDTO>> GetMyCart()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return BadRequest("No se pudo identificar al usuario");

            var cart = await _cartService.GetOrCreateByClientIdAsync(int.Parse(userIdClaim));
            if (cart == null)
                return NotFound($"No se pudo crear el Carro");

            return Ok(cart);
        }
    }
}
