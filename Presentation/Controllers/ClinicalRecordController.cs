using Application.Interfaces;
using Application.Models.ClinicalRecord;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicalRecordController : ControllerBase
    {
        private readonly IClinicalRecordService _clinicalRecordService;
        private readonly IPetService _petService;
        public ClinicalRecordController(IClinicalRecordService clinicalRecordService, IPetService petService)
        {
            _clinicalRecordService = clinicalRecordService;
            _petService = petService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Veterinario,Recepcionista")]
        public async Task<ActionResult<List<ClinicalRecordDTO>>> Get()
        {
            var clinicalRecords = await _clinicalRecordService.GetAllAsync();
            return Ok(clinicalRecords);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Veterinario,Recepcionista")]
        public async Task<ActionResult<ClinicalRecordDTO>> GetById([FromRoute] int id)
        {
            var clinicalRecord = await _clinicalRecordService.GetByIdAsync(id);
            if (clinicalRecord == null) 
                return NotFound("El registro clínico no existe");

            return Ok(clinicalRecord);
        }

        [HttpGet("pet/{petId}")]
        [Authorize(Roles = "Admin,Veterinario,Recepcionista,Cliente")]
        public async Task<ActionResult<List<ClinicalRecordDTO>>> GetByPetIdAsync([FromRoute] int petId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "Cliente")
            {
                if (userIdClaim == null) return Unauthorized();
                int currentUserId = int.Parse(userIdClaim);

                var pet = await _petService.GetByIdAsync(petId);
                if (pet == null)
                    return NotFound("La mascota no existe");

                if (pet.ClientId != currentUserId)
                {
                    return Forbid();
                }
            }

            var clinicalRecords = await _clinicalRecordService.GetByPetIdAsync(petId);
            return Ok(clinicalRecords);
        }

        [HttpGet("veterinary/{veterinaryId}")]
        [Authorize(Roles = "Admin,Veterinario,Recepcionista")]
        public async Task<ActionResult<List<ClinicalRecordDTO>>> GetByVeterinaryIdAsync([FromRoute] int veterinaryId)
        {
            var clinicalRecords = await _clinicalRecordService.GetByVeterinaryIdAsync(veterinaryId);
            return Ok(clinicalRecords);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Veterinario,Recepcionista")]
        public async Task<ActionResult<ClinicalRecordDTO>> Add([FromBody] CreateClinicalRecordDTO clinicalRecord)
        {
            var createdClinicalRecord = await _clinicalRecordService.AddAsync(clinicalRecord);
            if (createdClinicalRecord == null) return 
                    BadRequest("El registro clínico no pudo ser creado");

            return CreatedAtAction(
            actionName: nameof(GetById),
            routeValues: new {id = createdClinicalRecord.Id},
            value: createdClinicalRecord
            );
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Veterinario,Recepcionista")]
        public async Task<ActionResult<ClinicalRecordDTO>> Update([FromBody] ClinicalRecordDTO clinicalRecord, [FromRoute] int id)
        {
            if (clinicalRecord == null) 
                return BadRequest("Los datos del registro clínico están incompletos");
            if (clinicalRecord.Id != id) 
                return BadRequest("El Id de la ruta no coincide con el del registro clínico");

            var updateClinicalRecord = await _clinicalRecordService.UpdateAsync(clinicalRecord);
            if (updateClinicalRecord == null) 
                return NotFound($"El registro clínico con el Id:{clinicalRecord.Id} no pudo ser actualizado");

            return Ok(updateClinicalRecord);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Veterinario,Recepcionista")]
        public async Task<ActionResult<ClinicalRecordDTO>> Delete([FromRoute] int id)
        {
            var clinicalRecord = await _clinicalRecordService.DeleteAsync(id);
            if (clinicalRecord == null)
                return NotFound("El registro clínico no existe");
            
            return Ok(clinicalRecord);
        }
    }
}