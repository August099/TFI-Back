using Application.Interfaces;
using Application.Models.CartItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;
        private readonly ICartService _cartService;
        public CartItemController(ICartItemService cartItemService, ICartService cartService)
        {
            _cartItemService = cartItemService;
            _cartService = cartService;
        }

        [HttpGet("{cartId}")]
        public async Task<ActionResult<List<CartItemDTO>>> GetByCartId([FromRoute] int cartId)
        {
            var cartItems = await _cartItemService.GetByCartIdAsync(cartId);
            return Ok(cartItems);
        }

        [HttpGet("{cartId}/{id}")]
        public async Task<ActionResult<CartItemDTO>> GetById([FromRoute] int cartId, [FromRoute] int id)
        {
            var cartItem = await _cartItemService.GetByIdAsync(id);
            if (cartItem == null)
                return NotFound($"El CartItem con el Id {id} no existe");

            if (cartItem.CartId != cartId)
                return BadRequest("El CartItem no pertenece al Carro");

            return Ok(cartItem);
        }

        [HttpPost]
        public async Task<ActionResult<CartItemDTO>> Add([FromBody] CreateCartItemDTO cartItem)
        {
            var getCart = await _cartService.GetOrCreateByClientIdAsync(cartItem.CartId);
            if (getCart == null)
                return BadRequest($"No se pudo obtener el carrito con Id:{cartItem.CartId}");

            var createdCartItem = await _cartItemService.AddAsync(cartItem);
            if (createdCartItem == null)
                return BadRequest($"El CartItem con Id:{cartItem.ItemId} no se pudo crear");

            return CreatedAtAction(
                nameof(GetById),
                routeValues: new {cartId = createdCartItem.CartId ,Id = createdCartItem.Id },
                value: createdCartItem
                );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CartItemDTO>> Update([FromBody] CartItemDTO cartItem, [FromRoute] int id)
        {
            if (cartItem == null)
                return BadRequest("El CartItem no puede estar vacío");
            if (cartItem.Id != id)
                return BadRequest("El Id del CartItem no coincide con el de la ruta");

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(userRole == "Cliente" || userRole == "Veterinario")
            {
                if (string.IsNullOrEmpty(userIdClaim))
                    return BadRequest("No se pudo identificar al usuario");

                // validar que el item pertenezca al carro
                var cartItemDB = await _cartItemService.GetByIdAsync(id);
                if (cartItemDB == null)
                    return NotFound($"El CartItem con el Id:{id} no existe");

                if (cartItemDB.CartId != cartItem.CartId)
                    return Forbid();

                // validar que el carro pertenezca al usuario
                var cartDB = await _cartService.GetByIdAsync(cartItemDB.CartId);
                if (cartDB == null)
                    return NotFound($"No se pudo encontrar el Carro con Id:{cartItemDB.CartId}");

                if (cartDB.ClientId != int.Parse(userIdClaim))
                    return Forbid();
            }

            var updatedCartItem = await _cartItemService.UpdateAsync(cartItem);
            if (updatedCartItem == null)
                return NotFound($"No se encontró el CartItem con Id: {cartItem.Id}");

            return Ok(updatedCartItem);
        }
    }
}