using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Text;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public abstract class BaseService : IBaseService
    {
        private const string APPLICATION_JSON = "application/json";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(
            IHttpClientFactory httpClientFactory,
            ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("MangoAPI");
                var message = new HttpRequestMessage();
                message.Headers.Add("Accept", APPLICATION_JSON);

                if (withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }

                message.RequestUri = new Uri(requestDto.Url);
                if (requestDto.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, APPLICATION_JSON);
                }

                message.Method = requestDto.ApiType switch
                {
                    SD.ApiType.DELETE => HttpMethod.Delete,
                    SD.ApiType.POST => HttpMethod.Post,
                    SD.ApiType.PUT => HttpMethod.Put,
                    _ => HttpMethod.Get,
                };

                var apiResponse = await client.SendAsync(message);

                switch (apiResponse.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };
                        break;
                    case System.Net.HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Access Denied" };
                    case System.Net.HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not Found" };
                    case System.Net.HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internal Server Error" };
                    case System.Net.HttpStatusCode.BadRequest:
                        return new() { IsSuccess = false, Message = "Bad Request" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    Message = ex.Message,
                    IsSuccess = false,
                };
            }
        }
    }
}
