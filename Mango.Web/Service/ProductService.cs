using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class ProductService : BaseService, IProductService
    {
        public ProductService(
            IHttpClientFactory httpClientFactory,
            ITokenProvider tokenProvider)
            : base(httpClientFactory, tokenProvider)
        {
        }

        public async Task<ResponseDto?> CreateProductAsync(ProductDto productDto) =>
            await SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Url = SD.ProductAPIBase + "/api/product",
                Data = productDto,
            });

        public async Task<ResponseDto?> DeleteProductAsync(int id) =>
            await SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.ProductAPIBase + "/api/product/" + id,
            });

        public async Task<ResponseDto?> GetAllProductsAsync() =>
            await SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + "/api/product",
            });

        public async Task<ResponseDto?> GetProductAsync(int id) =>
            await SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + "/api/product/" + id,
            });

        public async Task<ResponseDto?> UpdateProductAsync(ProductDto productDto) =>
            await SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.PUT,
                Url = SD.ProductAPIBase + "/api/product",
                Data = productDto,
            });
    }
}
