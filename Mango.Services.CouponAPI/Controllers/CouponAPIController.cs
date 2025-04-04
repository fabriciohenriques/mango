using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private IMapper _mapper;
        private ResponseDto _response;

        public CouponAPIController(
            AppDbContext db,
            IMapper mapper
            )
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [HttpGet]
        public async Task<ResponseDto> Get()
        {
            try
            {
                var obj = await _db.Coupons.ToListAsync();
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpGet("{id:int}")]
        public async Task<ResponseDto> Get(int id)
        {
            try
            {
                var obj = await _db.Coupons.FirstAsync(c => c.CouponId == id);
                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpGet("GetByCode/{code}")]
        [AllowAnonymous]
        public async Task<ResponseDto> Get(string code)
        {
            try
            {
                var obj = await _db.Coupons.FirstAsync(c => c.CouponCode.ToLower() == code.ToLower());
                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpPost]
        public async Task<ResponseDto> Post([FromBody] CouponDto couponDto)
        {
            try
            {
                var obj = _mapper.Map<Coupon>(couponDto);
                await _db.Coupons.AddAsync(obj);
                await _db.SaveChangesAsync();

                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpPut]
        public async Task<ResponseDto> Put([FromBody] CouponDto couponDto)
        {
            try
            {
                var obj = _mapper.Map<Coupon>(couponDto);
                _db.Coupons.Update(obj);
                await _db.SaveChangesAsync();

                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpDelete("{id:int}")]
        public async Task<ResponseDto> Delete(int id)
        {
            try
            {
                var obj = _db.Coupons.First(c => c.CouponId == id);
                _db.Coupons.Remove(obj);
                await _db.SaveChangesAsync();
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
