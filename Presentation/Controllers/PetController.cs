using Application.Interfaces;
using Application.Models;
using Application.Models.Pet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PetController : ControllerBase
    {
        private readonly IPetService _petService;
        public PetController(IPetService petService)
        {
            _petService = petService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Recepcionista,Veterinario")]
        public async Task<ActionResult<List<PetDTO>>> Get()
        {
            var pets = await _petService.GetAllAsync();
            return Ok(pets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PetDTO>> GetById(int id)
        {
            var UserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var pet = await _petService.GetByIdAsync(id);
            if (pet == null) 
                return NotFound($"No se encontró la mascota con el Id: {id}");

            if (UserRole == "Cliente")
            {
                if (string.IsNullOrEmpty(userIdClaim))
                    return BadRequest("No se pudo identificar al usuario");

                if (pet.ClientId != int.Parse(userIdClaim))
                    return Forbid();
            }

            return Ok(pet);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Cliente,Recepcionista")]
        public async Task <ActionResult<PetDTO>> Add([FromBody] CreatePetDTO pet)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Cliente")
            {
                if (string.IsNullOrEmpty(userIdClaim))
                    return BadRequest("No se pudo identificar al usuario");

                if (pet.ClientId != int.Parse(userIdClaim))
                    return Forbid();
            }
            else
            {
                if (pet.ClientId <= 0)
                    return BadRequest("El Id del dueño de la mascota no es válido");
            }

            var createdPet = await _petService.AddAsync(pet);
            if (createdPet == null) 
                return BadRequest($"La mascota {pet.Name} no pudo ser creada");

            return CreatedAtAction(
                actionName: nameof(GetById),
                routeValues: new { id = createdPet.Id },
                value: createdPet
            );
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Cliente,Recepcionista")]
        public async Task<ActionResult<PetDTO>> Update([FromBody] PetDTO pet, [FromRoute] int id)
        {
            if (pet == null) 
                return BadRequest("Los datos de la mascota están incompletos");
            if (pet.Id != id) 
                return BadRequest("El Id de la ruta no coincide con el de la mascota");

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var dbPet = await _petService.GetByIdAsync(id);
            if (dbPet == null)
                return NotFound($"No se encontró la mascota con el Id: {id}");

            if (userRole == "Cliente")
            {
                if (string.IsNullOrEmpty(userIdClaim))
                    return BadRequest("No se pudo identificar al usuario");

                if (dbPet.ClientId != int.Parse(userIdClaim))
                    return Forbid();
            }

            if (pet.ClientId != dbPet.ClientId)
                return BadRequest("No se puede cambiar el dueño de la mascota");

            var updatePet = await _petService.UpdateAsync(pet);
            if (updatePet == null) 
                return NotFound($"La mascota con el Id: {pet.Id} no pudo ser actualizada");

            return Ok(updatePet);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Recepcionista")]
        public async Task<ActionResult<PetDTO>> Delete([FromRoute] int id)
        {
            var pet = await _petService.DeleteAsync(id);
            if (pet == null) 
                return NotFound($"La mascota con el Id: {id} no pudo ser eliminada");

            return Ok(pet);
        }
    }
}