using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
    public class CuponController : Controller
    {
        private readonly ICuponService _cuponservice;
        public CuponController(ICuponService cuponService)
        {
            _cuponservice = cuponService;       
        }
        public async Task<IActionResult> CuponIndex()
        {
            List<CuponDto>? list = new();

            ResponseDto? response = await _cuponservice.GetAllCupons();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CuponDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["Error"] = response.Message;
            }

            return View(list);
        }

        public async Task<IActionResult> CuponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CuponCreate(CuponDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _cuponservice.CreateCuponAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Coupon created successfully";
                    return RedirectToAction(nameof(CuponIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> CuponDelete(int CuponId)
        {
			ResponseDto? response = await _cuponservice.GetCuponByIdAsync(CuponId);

			if (response != null && response.IsSuccess)
			{
				CuponDto? cuponDto = JsonConvert.DeserializeObject<CuponDto>(Convert.ToString(response.Result));
                return View(cuponDto);
			}
			return RedirectToAction(nameof(CuponIndex));
        }

        [HttpPost]
        public async Task<IActionResult> CuponDelete(CuponDto cuponDto)
        {
            ResponseDto? response = await _cuponservice.DeleteCuponAsync(cuponDto.CuponId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon deleted successfully";
                return RedirectToAction(nameof(CuponIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(cuponDto);
        }
    }
}
