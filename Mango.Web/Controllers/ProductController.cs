using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productservice;
        public ProductController(IProductService productservice)
        {
            _productservice = productservice;       
        }
        public async Task<IActionResult> ProductIndex()
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

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productservice.CreateProductAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ProductDelete(int ProductId)
        {
			ResponseDto? response = await _productservice.GetProductByIdAsync(ProductId);

			if (response != null && response.IsSuccess)
			{
                ProductDto? ProductDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(ProductDto);
			}
			return RedirectToAction(nameof(ProductIndex));
        }

        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDto ProductDto)
        {
            ResponseDto? response = await _productservice.DeleteProductAsync(ProductDto.ProductId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product deleted successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(ProductDto);
        }

        public async Task<IActionResult> ProductEdit(int ProductId)
        {
            ResponseDto? response = await _productservice.GetProductByIdAsync(ProductId);

            if (response != null && response.IsSuccess)
            {
                ProductDto? ProductDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(ProductDto);
            }
            return RedirectToAction(nameof(ProductIndex));
        }

        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductDto ProductDto)
        {
            ResponseDto? response = await _productservice.UpdateProductAsync(ProductDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product edited successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(ProductDto);
        }
    }
}
