using Mango.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrder(CartDto CartDto);
        Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto);
        Task<ResponseDto?> ValidateStripeSession(int OrderHeaderId);
        Task<ResponseDto?> GetAllOrder(string? userId);
        Task<ResponseDto?> GetOrder(int orderId);
        Task<ResponseDto?> UpdateOrderStatus(int orderId, [FromBody] string newStatus);
    }
}
