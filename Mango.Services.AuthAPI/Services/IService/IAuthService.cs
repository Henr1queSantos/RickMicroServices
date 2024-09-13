using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;

namespace Mango.Services.AuthAPI.Services.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDTO registrationRequestDTO);
        Task<LoginResponseDto> Login(LoginRequestDTO loginRequestDTO);

        Task<bool> AssignRole(string email, string rolename);
    }
}
