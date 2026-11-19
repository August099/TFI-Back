using Domain.Entities;

namespace Application.Models.Pet
{
    public class CreatePetDTO
    {
        public int ClientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Race { get; set; } = string.Empty;
        public Gender Gender { get; set; } = Gender.Macho;
        public string Species { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
    }
}
