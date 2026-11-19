using Application.Interfaces;
using Application.Models.Shift;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ShiftController : ControllerBase
    {
        private readonly IShiftService _shiftService;

        public ShiftController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Veterinario,Recepcionista")]
        public async Task<ActionResult<List<ShiftDTO>>> Get()
        {
            var shifts = await _shiftService.GetAllAsync();
            return Ok(shifts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShiftDTO>> GetById([FromRoute] int id)
        {
            var shift = await _shiftService.GetByIdAsync(id);
            if (shift == null)
                return NotFound($"No se encontró el turno con Id: {id}");

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "Cliente")
            {
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
                    return Unauthorized("No se pudo identificar al usuario desde el token");

                if (shift.ClientId != currentUserId)
                    return Forbid();
            }

            return Ok(shift);
        }

        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<List<ShiftDTO>>> GetByClientIdAsync([FromRoute] int clientId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "Cliente")
            {
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
                    return Unauthorized("No se pudo identificar al usuario desde el token");

                if (currentUserId != clientId)
                    return Forbid();
            }

            var shifts = await _shiftService.GetByClientIdAsync(clientId);
            return Ok(shifts);
        }

        [HttpGet("veterinary/{veterinaryId}")]
        [Authorize(Roles = "Admin,Recepcionista,Veterinario")]
        public async Task<ActionResult<List<ShiftDTO>>> GetByVeterinaryIdAsync([FromRoute] int veterinaryId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "Veterinario")
            {
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
                    return Unauthorized("No se pudo identificar al veterinario desde el token");

                if (currentUserId != veterinaryId)
                    return Forbid();
            }

            var shifts = await _shiftService.GetByVeterinaryIdAsync(veterinaryId);
            return Ok(shifts);
        }

        [HttpGet("pet/{petId}")]
        public async Task<ActionResult<List<ShiftDTO>>> GetByPetIdAsync([FromRoute] int petId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var shifts = await _shiftService.GetByPetIdAsync(petId);

            if (userRole == "Cliente")
            {
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
                    return Unauthorized("No se pudo identificar al usuario desde el token");

                shifts = shifts.Where(s => s.ClientId == currentUserId).ToList();
            }

            return Ok(shifts);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<ShiftDTO>>> GetByStatusAsync([FromRoute] ShiftStatus status)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var shifts = await _shiftService.GetByStatusAsync(status);

            if (userRole == "Cliente")
            {
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
                    return Unauthorized("No se pudo identificar al usuario desde el token");

                shifts = shifts.Where(s => s.ClientId == currentUserId).ToList();
            }

            return Ok(shifts);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Recepcionista,Cliente")]
        public async Task<ActionResult<ShiftDTO>> Add([FromBody] CreateShiftDTO shift)
        {
            if (shift == null)
                return BadRequest("El turno no puede estar vacío");

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "Cliente")
            {
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
                    return Unauthorized("No se pudo identificar al cliente desde el token");

                shift.ClientId = currentUserId;
            }

            var createdShift = await _shiftService.AddAsync(shift);
            if (createdShift == null)
                return BadRequest("No se pudo crear el turno");

            return CreatedAtAction(
                actionName: nameof(GetById),
                routeValues: new { id = createdShift.Id },
                value: createdShift
            );
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Veterinario,Recepcionista")]
        public async Task<ActionResult<ShiftDTO>> Update([FromRoute] int id, [FromBody] ShiftDTO shift)
        {
            if (shift == null)
                return BadRequest("Los datos del turno están incompletos");
            if (shift.Id != id)
                return BadRequest("El Id de la ruta no coincide con el del turno");

            var updatedShift = await _shiftService.UpdateAsync(shift);
            if (updatedShift == null)
                return NotFound($"No se pudo actualizar el turno con Id: {shift.Id}");

            return Ok(updatedShift);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Recepcionista")]
        public async Task<ActionResult<ShiftDTO>> Delete([FromRoute] int id)
        {
            var shift = await _shiftService.DeleteAsync(id);
            if (shift == null)
                return NotFound($"El turno con el Id:{id} no pudo ser eliminado");

            return Ok(shift);
        }
    }
}