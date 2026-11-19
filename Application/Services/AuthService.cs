using Application.Interfaces;
using Application.Models.Auth;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private const string EmailPattern = @"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$";
        private const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$";

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task Register(RegisterAuthDTO request)
        {
            // 1) Verificar si ya existe un usuario con ese email
            var existingUserByEmail = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUserByEmail != null)
                throw new ConflictException("El email ya se encuentra registrado.");

            // 2) Verificar si ya existe un usuario con ese dni
            var existingUserByDni = await _userRepository.GetByDniAsync(request.Dni);
            if (existingUserByDni != null)
                throw new ConflictException("El dni ya se encuentra registrado.");

            // 3) Validar el email usando una expresión regular
            var email = request.Email.Trim().ToLowerInvariant();

            if (!Regex.IsMatch(email, EmailPattern))
                throw new BadRequestException("El correo no tiene un formato válido");

            var password = request.Password;

            // 4) validar el tamaño de la contraseña
            if (password.Length < 8)
                throw new BadRequestException("La contraseña debe tener al menos 8 caracteres.");

            // 5) Validar la contraseña usando una expresión regular

            if (!Regex.IsMatch(password, PasswordPattern))
                throw new BadRequestException("La contraseña debe tener una minuscula, una mayuscula y un numero.");

            // 6) Crear nuevo usuario mapeando todos los campos y hasheando la contraseña
            var user = new User
            {
                Dni = request.Dni,
                Name = request.Name,
                Email = request.Email,
                Telephone = request.Telephone,
                State = true,
                BirthDate = request.BirthDate,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Rol = Role.Cliente
            };

            // 7) Guardar usuario en la base de datos
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<AuthResponseDTO> Login(LoginRequestDTO request)
        {
            // 1) Buscar usuario por email
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new InvalidCredentialException();

            // 2) Verificar contraseña con BCrypt
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isValidPassword)
                throw new InvalidCredentialException();

            // 3) Generar token
            return GenerateToken(user);
        }

        private AuthResponseDTO GenerateToken(User user)
        {
            // 1) Crear claims usando el rol específico del usuario
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.GivenName, user.Name ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Rol.ToString())
            };

            // 2) Obtener configuración JWT
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key no está configurado");
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var jwtExpires = int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddMinutes(jwtExpires);

            // 3) Crear token
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // 4) Armar respuesta
            return new AuthResponseDTO
            {
                Token = tokenString,
                ExpiresAt = expiresAt,
                Email = user.Email ?? string.Empty,
                FullName = user.Name?.Trim() ?? string.Empty,
                Rol = user.Rol
            };
        }
    }
}