using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class CouponService : BaseService, ICouponService
    {
        public CouponService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public async Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto) =>
            await base.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Url = CouponAPIBase + "/api/coupon",
                Data = couponDto,
            });

        public async Task<ResponseDto?> DeleteCouponAsync(int id) =>
            await base.SendAsync(new RequestDto
            {
                ApiType = ApiType.DELETE,
                Url = CouponAPIBase + "/api/coupon/" + id,
            });

        public async Task<ResponseDto?> GetAllCouponsAsync() =>
            await base.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = CouponAPIBase + "/api/coupon",
            });

        public async Task<ResponseDto?> GetCouponAsync(string couponCode) =>
            await base.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = CouponAPIBase + "/api/coupon/getbycode/" + couponCode,
            });

        public async Task<ResponseDto?> GetCouponByIdAsync(int id) =>
            await base.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = CouponAPIBase + "/api/coupon/" + id,
            });

        public async Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto) =>
            await base.SendAsync(new RequestDto
            {
                ApiType = ApiType.PUT,
                Url = CouponAPIBase + "/api/coupon",
                Data = couponDto,
            });
    }
}
