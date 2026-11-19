using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Pet
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Race { get; set; } = string.Empty;
        public Gender Gender { get; set; } = Gender.Macho;
        public string Species { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
    }

    public enum Gender
    {
        Macho = 0,
        Hembra = 1
    }
}