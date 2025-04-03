using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
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
        private readonly IMapper _mapper;

        public CartAPIController(
            AppDbContext db,
            IMapper mapper)
        {
            _response = new ResponseDto();
            _db = db;
            _mapper = mapper;
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

        [HttpPost("RemoveCartDetail")]
        public async Task<IActionResult> RemoveCartDetail([FromBody] int carDetailId)
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
