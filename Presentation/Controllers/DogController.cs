using Application.Interfaces;
using Application.Models;
using Application.Models.Dog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DogController : ControllerBase
    {
        private readonly IDogServices _dogServices;

        public DogController(IDogServices dogServices)
        {
            _dogServices = dogServices;
        }

        [HttpGet("breeds")]
        public async Task<ActionResult<List<BreedDTO>>> GetBreedListAsync()
            => Ok(await _dogServices.GetBreedListAsync());

        [HttpGet("breeds/{breed}")]
        public async Task<ActionResult<List<SubBreedDTO>>> GetSubBreedListAsync(string breed)
        {
            bool exists = await _dogServices.BreedExistsAsync(breed);
            if (!exists)
            {
                return BadRequest("La raza no existe.");
            }

            return Ok(await _dogServices.GetSubBreedListAsync(breed));
        }

        [HttpGet("random")]
        public async Task<ActionResult<DogImageDTO>> GetRandomDogAsync()
            => Ok(await _dogServices.GetRandomDogAsync());

        [HttpGet("random/{cantidad:int}")]
        public async Task<ActionResult<List<DogImageDTO>>> GetMultipleRandomDogAsync(int cantidad)
        {
            if (cantidad < 1)
                return BadRequest("La cantidad debe ser mayor a 0.");

            return Ok(await _dogServices.GetMultipleRandomDogAsync(cantidad));
        }

        [HttpGet("all/{breed}")]
        public async Task<ActionResult<List<DogImageDTO>>> GetDogsByBreedAsync(string breed)
        {
            bool exists = await _dogServices.BreedExistsAsync(breed);
            if (!exists)
            {
                return BadRequest("La raza no existe.");
            }

            return Ok(await _dogServices.GetDogsByBreedAsync(breed));
        }

        [HttpGet("random/{breed}")]
        public async Task<ActionResult<DogImageDTO>> GetRandomDogByBreedAsync(string breed)
        {
            bool exists = await _dogServices.BreedExistsAsync(breed);
            if (!exists)
            {
                return BadRequest("La raza no existe.");
            }

            return Ok(await _dogServices.GetRandomDogByBreedAsync(breed));
        }

        [HttpGet("random/{breed}/{cantidad:int}")]
        public async Task<ActionResult<List<DogImageDTO>>> GetMultipleRandomDogByBreedAsync(string breed, int cantidad)
        {
            bool exists = await _dogServices.BreedExistsAsync(breed);
            if (!exists)
            {
                return BadRequest("La raza no existe.");
            }

            if (cantidad < 1)
                return BadRequest("La cantidad debe ser mayor a 0.");

            return Ok(await _dogServices.GetMultipleRandomDogByBreedAsync(breed, cantidad));
        }

        [HttpGet("all/{breed}/{subBreed}")]
        public async Task<ActionResult<List<DogImageDTO>>> GetDogsBySubBreedAsync(string breed, string subBreed)
        {
            bool breedExists = await _dogServices.BreedExistsAsync(breed);
            if (!breedExists)
            {
                return BadRequest("La raza no existe.");
            }

            bool subBreedExists = await _dogServices.SubBreedExistsAsync(breed, subBreed);
            if (!subBreedExists)
            {
                return BadRequest("La subraza no existe.");
            }
            return Ok(await _dogServices.GetDogsBySubBreedAsync(breed, subBreed));
        }

        [HttpGet("random/{breed}/{subBreed}")]
        public async Task<ActionResult<List<DogImageDTO>>> GetRandomDogBySubBreedAsync(string breed, string subBreed)
        {
            bool breedExists = await _dogServices.BreedExistsAsync(breed);
            if (!breedExists)
            {
                return BadRequest("La raza no existe.");
            }

            bool subBreedExists = await _dogServices.SubBreedExistsAsync(breed, subBreed);
            if (!subBreedExists)
            {
                return BadRequest("La subraza no existe.");
            }
            return Ok(await _dogServices.GetRandomDogBySubBreedAsync(breed, subBreed));
        }

        [HttpGet("random/{breed}/{subBreed}/{cantidad:int}")]
        public async Task<ActionResult<List<DogImageDTO>>> GetMultipleRandomDogBySubBreedAsync(string breed, string subBreed, int cantidad)
        {
            bool breedExists = await _dogServices.BreedExistsAsync(breed);
            if (!breedExists)
            {
                return BadRequest("La raza no existe.");
            }

            bool subBreedExists = await _dogServices.SubBreedExistsAsync(breed, subBreed);
            if (!subBreedExists)
            {
                return BadRequest("La subraza no existe.");
            }

            if (cantidad < 1)
                return BadRequest("La cantidad debe ser mayor a 0.");

            return Ok(await _dogServices.GetMultipleRandomDogBySubBreedAsync(breed, subBreed, cantidad));
        }
    }
}
