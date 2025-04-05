using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private ResponseDto _response;
        private readonly AppDbContext _db;
        private readonly ICouponService _couponService;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public CartAPIController(
            AppDbContext db,
            ICouponService couponService,
            IMapper mapper,
            IProductService productService)
        {
            _response = new ResponseDto();
            _db = db;
            _couponService = couponService;
            _mapper = mapper;
            _productService = productService;
        }

        [HttpPost("applycoupon")]
        public async Task<ResponseDto> ApplyCoupon([FromBody] CartHeaderDto cartHeaderDto)
        {
            try
            {
                var cartDb = await _db.CartHeaders.FirstOrDefaultAsync(ch => ch.UserId == cartHeaderDto.UserId);
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

        [HttpGet("getcart/{userId}")]
        public async Task<ResponseDto> GetCart(Guid userId)
        {
            try
            {
                var cartDb = await _db.CartHeaders.Include(ch => ch.CartDetails).FirstOrDefaultAsync(ch => ch.UserId == userId);

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

                    cartDto.CartHeader.Discount = coupon.DiscountAmount;
                    if (coupon != null && cartDto.CartHeader.CartTotal >= coupon.MinAmount)
                        cartDto.CartHeader.CartTotal -= cartDto.CartHeader.CartTotal * coupon.DiscountAmount / 100;
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
        [HttpPost]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeaders.FirstOrDefaultAsync(c => c.UserId == cartDto.CartHeader.UserId);

                if (cartHeaderFromDb == null)
                {
                    //create header and detail
                    var cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
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

        [HttpDelete("removecartdetail/{carDetailId: int}")]
        public async Task<ResponseDto> RemoveCartDetail(int carDetailId)
        {
            try
            {
                var cartDetail = await _db.CartDetails.FirstOrDefaultAsync(cd => cd.CartDetailsId == carDetailId);

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

                _response.Message = "Cart Detail not found.";
                _response.IsSuccess = false;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }
    }
}
