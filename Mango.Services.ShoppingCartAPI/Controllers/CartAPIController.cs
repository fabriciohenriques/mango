using AutoMapper;
using Mango.ServiceBus;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize]
    public class CartAPIController : ControllerBase
    {
        private ResponseDto _response;
        private readonly AppDbContext _db;
        private readonly ICouponService _couponService;
        private readonly IMapper _mapper;
        private readonly IMessageBus _messageBus;
        private readonly IProductService _productService;
        private readonly ServiceBusConfig _serviceBusConfig;

        public CartAPIController(
            AppDbContext db,
            ICouponService couponService,
            IMapper mapper,
            IMessageBus messageBus,
            IProductService productService,
            IOptions<ServiceBusConfig> serviceBusConfigOptions)
        {
            _response = new ResponseDto();
            _db = db;
            _couponService = couponService;
            _mapper = mapper;
            _messageBus = messageBus;
            _productService = productService;
            _serviceBusConfig = serviceBusConfigOptions.Value;
        }

        [HttpPost("applycoupon")]
        public async Task<ResponseDto> ApplyCoupon([FromBody] CartHeaderDto cartHeaderDto)
        {
            try
            {
                var userId = GetUserId();
                var cartDb = await _db.CartHeaders.FirstOrDefaultAsync(ch => ch.UserId == userId);
                if (cartDb != null)
                {
                    cartDb.CouponCode = string.IsNullOrEmpty(cartHeaderDto.CouponCode) ? default : cartHeaderDto.CouponCode;
                    _db.CartHeaders.Update(cartDb);
                    await _db.SaveChangesAsync();
                    _response.IsSuccess = true;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Not found.";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpGet("getcart")]
        public async Task<ResponseDto> GetCart()
        {
            try
            {
                var cartDto = await GetCartForUser();

                if (cartDto == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Cart not found.";
                    return _response;
                }

                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }
        [HttpPost("cartupsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var userId = GetUserId();
                var cartHeaderFromDb = await _db.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);

                if (cartHeaderFromDb == null)
                {
                    //create header and detail
                    var cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    cartHeader.UserId = userId;
                    var cartHeaderToDb = await _db.AddAsync(cartHeader);
                    var cartDetail = _mapper.Map<CartDetail>(cartDto.CartDetails.First());
                    cartDetail.CartHeader = cartHeader;
                    await _db.AddAsync(cartDetail);

                    await _db.SaveChangesAsync();
                }
                else
                {
                    var cartDetailsFromDb = await _db.CartDetails.FirstOrDefaultAsync(cd => cd.ProductId == cartDto.CartDetails.First().ProductId && cd.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        var cartDetail = _mapper.Map<CartDetail>(cartDto.CartDetails.First());
                        cartDetail.CartHeader = cartHeaderFromDb;
                        await _db.AddAsync(cartDetail);

                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        cartDetailsFromDb.Count = cartDto.CartDetails.First().Count;
                        _db.Update(cartDetailsFromDb);
                        await _db.SaveChangesAsync();
                    }
                }

                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }

        [HttpDelete("removecartdetail/{carDetailId:int}")]
        public async Task<ResponseDto> RemoveCartDetail(int carDetailId)
        {
            try
            {
                var cartDetail = await _db.CartDetails.Include(cd => cd.CartHeader).FirstOrDefaultAsync(cd => cd.CartDetailsId == carDetailId);

                var userId = GetUserId();

                if (cartDetail?.CartHeader.UserId != userId)
                {
                    _response.Message = "Unauthorized user.";
                    _response.IsSuccess = false;
                    return _response;
                }

                if (cartDetail != null)
                {
                    _db.Remove(cartDetail);

                    var totalDetailsOfCart = await _db.CartDetails.CountAsync(cd => cd.CartHeaderId == cartDetail.CartHeaderId);

                    if (totalDetailsOfCart == 1)
                    {
                        var cartHeader = await _db.CartHeaders.FirstAsync(ch => ch.CartHeaderId == cartDetail.CartHeaderId);
                        _db.Remove(cartHeader);
                    }
                    await _db.SaveChangesAsync();

                    _response.IsSuccess = true;
                }
                else
                {
                    _response.Message = "Cart Detail not found.";
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }

        [HttpPost("emailcartrequest")]
        public async Task<ResponseDto> EmailCartRequest()
        {
            try
            {
                var cartDto = await GetCartForUser();

                if (cartDto == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Cart not found.";
                    return _response;
                }

                var userCartHeaderDto = new
                {
                    cartDto.CartHeader,
                    cartDto.CartDetails,
                    Email = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Email)?.Value,
                };
                await _messageBus.PublishMessage(userCartHeaderDto, _serviceBusConfig.EmailShoppingCart, _serviceBusConfig.ConnectionString);
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }

        private async Task<CartDto?> GetCartForUser()
        {
            var userId = GetUserId();

            var cartDb = await _db.CartHeaders.Include(ch => ch.CartDetails).FirstOrDefaultAsync(ch => ch.UserId == userId);

            if (cartDb == null)
            {
                return null;
            }

            var products = await _productService.GetProducts();

            var cartDto = new CartDto
            {
                CartHeader = _mapper.Map<CartHeaderDto>(cartDb),
                CartDetails = _mapper.Map<IEnumerable<CartDetailDto>>(cartDb.CartDetails),
            };

            foreach (var item in cartDto.CartDetails)
            {
                item.Product = products.FirstOrDefault(p => p.ProductId == item.ProductId);
                cartDto.CartHeader.CartTotal += item.Count * item.Product.Price;
            }

            if (!string.IsNullOrEmpty(cartDb.CouponCode))
            {
                var coupon = await _couponService.GetCouponByCode(cartDb.CouponCode);

                if (coupon != null && cartDto.CartHeader.CartTotal >= coupon.MinAmount)
                {
                    cartDto.CartHeader.Discount = cartDto.CartHeader.CartTotal * coupon.DiscountAmount / 100;
                    cartDto.CartHeader.CartTotal -= cartDto.CartHeader.Discount;
                }
            }

            return cartDto;
        }

        private Guid GetUserId() =>
            new Guid(User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value);
    }
}
