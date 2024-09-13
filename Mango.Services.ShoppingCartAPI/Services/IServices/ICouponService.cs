using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Services.IServices
{
    public interface ICouponService
    {
        Task<CuponDto> GetCoupon(string couponCode);
    }
}
