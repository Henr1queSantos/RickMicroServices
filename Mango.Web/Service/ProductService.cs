using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class productService : IProductService
    {
        private readonly IBaseService _baseService;

        public productService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> CreateProductAsync(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Post,
                Url = SD.productAPIBase + "/api/product",
                Data = productDto,
                ContentType = SD.ContentType.MultipartFormData
            });
        }

        public async Task<ResponseDto?> DeleteProductAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Delete,
                Url = SD.productAPIBase + "/api/product/" + id
            });
        }

        public async Task<ResponseDto?> GetAllProduct()
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Get,
                Url = SD.productAPIBase + "/api/product"
            });
        }

        public async Task<ResponseDto?> GetProductAsync(string productCode)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Get,
                Url = SD.productAPIBase + "/api/product/GetByCode/" + productCode
            });
        }

        public async Task<ResponseDto?> GetProductByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Get,
                Url = SD.productAPIBase + "/api/product/" + id
            });
        }

        public async Task<ResponseDto?> UpdateProductAsync(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Put,
                Url = SD.productAPIBase + "/api/product",
                Data = productDto,
                ContentType = SD.ContentType.MultipartFormData
            });
        }
    }
}
