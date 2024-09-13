using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Newtonsoft.Json;
using System.Net.Http;

namespace Mango.Services.ShoppingCartAPI.Services
{
    public class CouponService : ICouponService
    {

        private readonly IHttpClientFactory _httpClientFactory;
        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<CuponDto> GetCoupon(string couponCode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/cupon/GetByCode/{couponCode}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess == true)
            {
                return JsonConvert.DeserializeObject<CuponDto>(Convert.ToString(resp.Result));
            }
            return new CuponDto();
        }
    }
}
