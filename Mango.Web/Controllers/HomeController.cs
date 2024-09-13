using IdentityModel;
using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly IProductService _productservice;
        private readonly ICartService _Cartservice;
        public HomeController(IProductService productservice, ICartService cartservice)
        {
            _productservice = productservice;
            _Cartservice = cartservice;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto>? list = new();

            ResponseDto? response = await _productservice.GetAllProduct();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["Error"] = response.Message;
            }

            return View(list);
        }

        [Authorize]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ProductDto? model = new();

            ResponseDto? response = await _productservice.GetProductByIdAsync(productId);

            if (response != null && response.IsSuccess)
            {
                model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["Error"] = response.Message;
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDto productDto)
        {
            CartDto? cartDto = new CartDto()
            {
                CartHeader = new CartHeaderDto
                {
                    UserId = User.Claims.Where(x => x.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDto cartDetailsDto = new CartDetailsDto()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId,

            };
            List<CartDetailsDto> cartDetailsDtos = new() { cartDetailsDto };

            cartDto.CartDetails = cartDetailsDtos;

            ResponseDto? response = await _Cartservice.UpsertCartAsync(cartDto);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Item has been added to the Shopping Cart";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = response.Message;
            }

            return View(productDto);
        }

        [Authorize (Roles = SD.RoleAdmin)]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
