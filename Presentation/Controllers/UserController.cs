using Application.Interfaces;
using Application.Models;
using Application.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/Users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Recepcionista")]
        public async Task<ActionResult<List<UserDTO>>> Get()
        {
            var Users = await _userService.GetAllAsync();
            return Ok(Users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetById([FromRoute] int id)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null) return Unauthorized();
            int currentUserId = int.Parse(userIdClaim);

            // para no ver perfiles de otros usuarios
            if ((userRole == "Cliente" || userRole == "Veterinario") && currentUserId != id)
            {
                return Forbid();
            }

            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound($"No se encontró el usuario con el Id: {id}");

            return Ok(user);
        }

        // Buscar por nombre
        [HttpGet("nombre/{name}")]
        [Authorize(Roles = "Admin,Recepcionista")]
        public async Task<ActionResult<UserDTO>> GetByName([FromRoute] string name)
        {
            var user = await _userService.GetByNameAsync(name);
            if(user == null) 
                return NotFound($"No se encontró el usuario con el nombre: {name}");

            return Ok(user);
        }

        // Actualizar
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDTO>> Put([FromBody] UserDTO user, [FromRoute] int id)
        {
            if (user == null) 
                return BadRequest("Los datos del usuario están incompletos");
            if (user.Id != id ) 
                return BadRequest("El Id de la ruta no coincide con el del usuario");

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null) 
                return Unauthorized(); // error 401
            int currentUserId = int.Parse(userIdClaim);

            if ((userRole == "Cliente" || userRole == "Veterinario") && currentUserId != id)
            {
                return Forbid(); // error 403
            }

            var updateUser = await _userService.UpdateAsync(user);
            if (updateUser == null) 
                return NotFound($"El usuario con el Id: {id} no pudo ser actualizado"); 

            return Ok(updateUser);
        }

        // Eliminar
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> Delete([FromRoute] int id)
        {
            var user = await _userService.DeleteAsync(id);
            if(user == null) 
                return NotFound($"El usuario con el Id: {id} no pudo ser eliminado");

            return Ok(user);
        }
    }
}