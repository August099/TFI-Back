using Application.Models.Auth;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task Register(RegisterAuthDTO request);
        Task<AuthResponseDTO> Login(LoginRequestDTO request);
    }
}