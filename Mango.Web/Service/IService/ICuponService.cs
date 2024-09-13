using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface ICuponService
    {
        Task<ResponseDto?> GetCuponAsync(string CuponCode);
        Task<ResponseDto?> GetAllCupons();
        Task<ResponseDto?> GetCuponByIdAsync(int id);
        Task<ResponseDto?> CreateCuponAsync(CuponDto cuponDto);
        Task<ResponseDto?> UpdateCuponAsync(CuponDto cuponDto);
        Task<ResponseDto?> DeleteCuponAsync (int id);

    }
}
