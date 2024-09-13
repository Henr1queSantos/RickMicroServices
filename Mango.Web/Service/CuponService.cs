using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class CuponService : ICuponService
    {
        private readonly IBaseService _baseService;

        public CuponService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> CreateCuponAsync(CuponDto cuponDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Post,
                Url = SD.CuponAPIBase + "/api/cupon",
                Data = cuponDto
            });
        }

        public async Task<ResponseDto?> DeleteCuponAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Delete,
                Url = SD.CuponAPIBase + "/api/cupon/" + id
            });
        }

        public async Task<ResponseDto?> GetAllCupons()
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Get,
                Url = SD.CuponAPIBase+"/api/cupon"
            });
        }

        public async Task<ResponseDto?> GetCuponAsync(string CuponCode)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Get,
                Url = SD.CuponAPIBase + "/api/cupon/GetByCode/" + CuponCode
            });
        }

        public async Task<ResponseDto?> GetCuponByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Get,
                Url = SD.CuponAPIBase + "/api/cupon/" + id
            });
        }

        public async Task<ResponseDto?> UpdateCuponAsync(CuponDto cuponDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.Put,
                Url = SD.CuponAPIBase + "/api/cupon",
                Data = cuponDto
            });
        }
    }
}
