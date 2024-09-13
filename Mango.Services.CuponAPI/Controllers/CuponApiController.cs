using AutoMapper;
using Mango.Services.CuponAPI.Data;
using Mango.Services.CuponAPI.Models;
using Mango.Services.CuponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Emit;

namespace Mango.Services.CuponAPI.Controllers
{
    [Route("api/cupon")]
    [ApiController]
    [Authorize]
    public class CuponApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;

        public CuponApiController(AppDbContext db, IMapper mapper) {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Cupon> objList = _db.Cupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<CuponDto>>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route ("{Id:int}")]
        public ResponseDto Get(int Id)
        {
            try
            {
                Cupon obj = _db.Cupons.First(x => x.CuponId == Id);
                _response.Result = _mapper.Map<CuponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("GetByCode/{Code}")]
        public ResponseDto GetByCode(string Code)
        {
            try
            {
                Cupon obj = _db.Cupons.First(x => x.CuponCode == Code);
                _response.Result = _mapper.Map<CuponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles ="ADMIN")]
        public ResponseDto Post([FromBody] CuponDto cuponDto)
        {
            try
            {
                Cupon obj = _mapper.Map<Cupon>(cuponDto);
                _db.Cupons.Add(obj);
                _db.SaveChanges();
                _response.Result = _mapper.Map<CuponDto>(obj);

                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff = (long)(cuponDto.DiscountAmount*100),
                    Name = cuponDto.CuponCode,
                    Currency = "eur",
                    Id = cuponDto.CuponCode
                };
                var service = new Stripe.CouponService();
                service.Create(options);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] CuponDto cuponDto)
        {
            try
            {
                Cupon obj = _mapper.Map<Cupon>(cuponDto);
                _db.Cupons.Update(obj);
                _db.SaveChanges();
                _response.Result = _mapper.Map<CuponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Cupon obj = _db.Cupons.First(x => x.CuponId == id);
                _db.Cupons.Remove(obj);
                _db.SaveChanges();

                
                var service = new Stripe.CouponService();
                service.Delete(obj.CuponCode);

                _response.Result = _mapper.Map<CuponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
