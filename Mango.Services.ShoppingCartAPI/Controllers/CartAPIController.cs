﻿using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
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
                    var cartDetailToDb = await _db.AddAsync(cartDetail);

                    await _db.SaveChangesAsync();
                }
                else
                {
                    var cartDetailsFromDb = await _db.CartDetails.FirstAsync(cd => cd.ProductId == cartDto.CartDetails.First().ProductId && cd.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        //create cartdetail
                    }
                    else
                    {
                        //update count
                    }
                }
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
