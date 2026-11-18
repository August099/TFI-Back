using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public Role Rol { get; set; }
        public string Dni { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public bool State { get; set; } = true;
        public DateOnly BirthDate { get; set; }
    }

    public enum Role
    {
        Cliente = 0,
        Veterinario = 1,
        Recepcionista = 2,
        Admin = 3
    }
}