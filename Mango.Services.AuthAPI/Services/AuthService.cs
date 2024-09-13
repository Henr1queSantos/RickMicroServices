using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly RoleManager<IdentityRole> _RoleMaganer;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _UserManager = userManager;
            _RoleMaganer = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<bool> AssignRole(string email, string rolename)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());

            if (user != null)
            {
                if (!_RoleMaganer.RoleExistsAsync(rolename).GetAwaiter().GetResult())
                {
                    _RoleMaganer.CreateAsync(new IdentityRole(rolename)).GetAwaiter().GetResult();
                }

                await _UserManager.AddToRoleAsync(user, rolename);
                return true;
            }

            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName == loginRequestDTO.UserName.ToLower());

            bool isValid = await _UserManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (isValid == false || user == null)
            {
                return new LoginResponseDto()
                {
                    User = null,
                    Token = ""
                };
            }

            var roles = await _UserManager.GetRolesAsync(user);

            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDTO userDTO = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
            };

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDTO,
                Token = token
            };
            return loginResponseDto;

        }

        public async Task<string> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDTO.Email,
                Email = registrationRequestDTO.Email,
                NormalizedEmail = registrationRequestDTO.Email.ToUpper(),
                Name = registrationRequestDTO.Name,
                PhoneNumber = registrationRequestDTO.PhoneNumber,
            };

            try
            {
                var result = await _UserManager.CreateAsync(user, registrationRequestDTO.Password);
                if (result.Succeeded)
                {
                    var UsertoReturn = _db.ApplicationUsers.First(x => x.UserName == registrationRequestDTO.Email);

                    UserDTO Userto = new()
                    {
                        Email = UsertoReturn.Email,
                        Name = UsertoReturn.Name,
                        ID = UsertoReturn.Id,
                        PhoneNumber = UsertoReturn.PhoneNumber
                    };

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {

            }

            return "Error Encontered";

        }
    }
}
