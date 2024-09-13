using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _TokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _TokenProvider = tokenProvider; 
        }
        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new();

            return View(loginRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto obj)
        {
            ResponseDto responseDto = await _authService.LoginAsync(obj);
            ResponseDto AssignRole;

            if (responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));
                TempData["success"] = "Login Successful";
                await SignInUser(loginResponseDto);
                _TokenProvider.SetToken(loginResponseDto.Token);
                return RedirectToAction("Index", "Home");
            }

            TempData["error"] = responseDto.Message;

            return View(obj);
        }


        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem {Text=SD.RoleAdmin, Value = SD.RoleAdmin},
                new SelectListItem {Text=SD.RoleCustomer, Value = SD.RoleCustomer}
            };

            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto obj)
        {
            ResponseDto result = await _authService.RegisterAsync(obj);
            ResponseDto AssignRole;

            if (result != null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(obj.Role))
                {
                    obj.Role = SD.RoleCustomer;
                }
                AssignRole = await _authService.AssignRoleAsync(obj);

                if (AssignRole != null && AssignRole.IsSuccess)
                {
                    TempData["success"] = "Registration Successful";
                }
                return RedirectToAction(nameof(Login));
            }

            TempData["error"] = result.Message;

            var roleList = new List<SelectListItem>()
            {
                new SelectListItem {Text=SD.RoleAdmin, Value = SD.RoleAdmin},
                new SelectListItem {Text=SD.RoleCustomer, Value = SD.RoleCustomer}
            };

            return View(obj);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _TokenProvider.ClearToken();
            return RedirectToAction("Index","Home");
        }

        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(x => x.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
