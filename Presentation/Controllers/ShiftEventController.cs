using Application.Interfaces;
using Application.Models.ShiftEvent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Admin,Veterinario,Recepcionista")]
    [ApiController]
    [Route("api/[controller]")]
    public class ShiftEventController : ControllerBase
    {
        private readonly IShiftEventService _shiftEventService;
        private readonly IShiftService _shiftService;

        public ShiftEventController(IShiftEventService shiftEventService, IShiftService shiftService)
        {
            _shiftEventService = shiftEventService;
            _shiftService = shiftService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShiftEventDTO>> GetById([FromRoute] int id)
        {
            var shiftEvent = await _shiftEventService.GetByIdAsync(id);
            if (shiftEvent == null)
                return NotFound($"No se encontró el historial de turno con Id: {id}");
            return Ok(shiftEvent);
        }

        [HttpGet("shift/{shiftId}")]
        public async Task<ActionResult<List<ShiftEventDTO>>> GetByShiftId([FromRoute] int shiftId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "Cliente")
            {
                if (userIdClaim == null) return Unauthorized();
                int currentUserId = int.Parse(userIdClaim);

                var shift = await _shiftService.GetByIdAsync(shiftId);
                if (shift == null)
                    return NotFound("El turno no existe");

                if (shift.ClientId != currentUserId)
                {
                    return Forbid();
                }
            }

            var events = await _shiftEventService.GetByShiftIdAsync(shiftId);
            return Ok(events);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<ShiftEventDTO>>> GetByUserId([FromRoute] int userId)
        {
            var events = await _shiftEventService.GetByUserIdAsync(userId);
            return Ok(events);
        }

        [HttpPost]
        public async Task<ActionResult<ShiftEventDTO>> AddAsync(CreateShiftEventDTO shiftEvent)
        {
            if (shiftEvent == null)
                return BadRequest("Los datos del evento son inválidos");

            var addedShiftEvent = await _shiftEventService.AddAsync(shiftEvent);
            if (addedShiftEvent == null)
                return BadRequest("No se pudo crear el evento del turno");

            return CreatedAtAction(
                actionName: nameof(GetById),
                routeValues: new { Id = addedShiftEvent.Id },
                value: addedShiftEvent
            );
        }
    }
}