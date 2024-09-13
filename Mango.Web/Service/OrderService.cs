using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;

        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> CreateOrder(CartDto CartDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Post,
                Url = SD.OrderAPIBase + "/api/order/CreateOrder",
                Data = CartDto
            });
        }

        public async Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Post,
                Url = SD.OrderAPIBase + "/api/order/CreateStripeSession",
                Data = stripeRequestDto
            });
        }

        public async Task<ResponseDto?> GetAllOrder(string? userId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Get,
                Url = SD.OrderAPIBase + "/api/order/GetOrders/",
                Data = userId
            });
        }

        public async Task<ResponseDto?> GetOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Get,
                Url = SD.OrderAPIBase + "/api/order/GetOrder/"+orderId,
                Data = orderId
            });
        }

        public async Task<ResponseDto?> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Post,
                Url = SD.OrderAPIBase + "/api/order/UpdateOrderStatus/"+orderId,
                Data = newStatus
            });
        }

        public async Task<ResponseDto?> ValidateStripeSession(int OrderHeaderId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Post,
                Url = SD.OrderAPIBase + "/api/order/ValidateStripeSession",
                Data = OrderHeaderId
            });
        }
    }
}
