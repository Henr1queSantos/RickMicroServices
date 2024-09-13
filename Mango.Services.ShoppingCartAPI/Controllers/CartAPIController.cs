using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.PortableExecutable;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private readonly IProductService _ProductService;
        private readonly ICouponService _CouponService;
        private readonly IMessageBus _MessageBus;
        private readonly IConfiguration _Configuration;

        public CartAPIController(AppDbContext db, IMapper mapper, IProductService productService, ICouponService CouponService, IMessageBus messageBus, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            this._response = new ResponseDto();
            _ProductService = productService;
            _CouponService = CouponService;
            _MessageBus = messageBus;
            _Configuration = configuration;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_db.CartHeaders.First(x => x.UserId == userId))
                };

                cartDto.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails.Where(x => x.CartHeaderId == cartDto.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> productDtos = await _ProductService.GetProducts();

                foreach (var item in cartDto.CartDetails)
                {
                    item.ProductDto = productDtos.FirstOrDefault(x => x.ProductId == item.ProductId);
                    cartDto.CartHeader.CartTotal += (item.ProductDto.Price * item.Count);
                }

                //apply coupon if any
                if (!string.IsNullOrEmpty(cartDto.CartHeader.CuponCode))
                {
                    CuponDto coupon = await _CouponService.GetCoupon(cartDto.CartHeader.CuponCode);

                    if (coupon != null && cartDto.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cartDto.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cartDto.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }

                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstAsync(x => x.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CuponCode = cartDto.CartHeader.CuponCode;
                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("EmailCartRequest")]
        public async Task<object> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {
                await _MessageBus.PublishMessage(cartDto, _Configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstAsync(x => x.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CuponCode = "";
                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //create header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();

                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //if header is not null
                    //check if details has same product
                    var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == cartDto.CartDetails.First().ProductId &&
                    x.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                    if (cartDetailsFromDb == null)
                    {
                        //create cartDetails
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        //update count in cartDetails
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }

                    _response.Result = cartDto;
                }
                return _response;
            }
            catch (Exception ex)
            {
                _response.Result = false;
                _response.Message = ex.Message;
                return _response;
            }
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _db.CartDetails.First(x => x.CartDetailsId == cartDetailsId);

                int totalCountOfCartItem = _db.CartDetails.Where(x => x.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);
                if (totalCountOfCartItem == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(x => x.CartHeaderId == cartDetails.CartHeaderId);

                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();
                _response.Result = true;
                return _response;
            }
            catch (Exception ex)
            {
                _response.Result = false;
                _response.Message = ex.Message;
                return _response;
            }
        }
    }
}
