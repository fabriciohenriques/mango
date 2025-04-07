using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class CartService : BaseService, ICartService
    {
        public CartService(
            IHttpClientFactory httpClientFactory,
            ITokenProvider tokenProvider)
            : base(httpClientFactory, tokenProvider)
        {
        }

        public async Task<ResponseDto?> ApplyCouponAsync(CartHeaderDto cartHeaderDto) =>
            await SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Data = cartHeaderDto,
                Url = SD.CartAPIBase + "/api/cart/applycoupon",
            });

        public async Task<ResponseDto?> GetCartByUserAsync() =>
            await SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CartAPIBase + $"/api/cart/getcart",
            });

        public async Task<ResponseDto?> RemoveFromCartAsync(int cartDetailId) =>
            await SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.DELETE,
                Data = cartDetailId,
                Url = SD.CartAPIBase + $"/api/cart/removecartdetail/{cartDetailId}",
            });

        public async Task<ResponseDto?> UpsertCartAsync(CartDto cartDto) =>
            await SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.CartAPIBase + "/api/cart/cartupsert",
            });

        public async Task<ResponseDto?> EmailCartAsync(CartDto cartDto) =>
            await SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.CartAPIBase + $"/api/cart/emailcartrequest",
            });
    }
}
