using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDto?> GetCartByUserAsync();
        Task<ResponseDto?> UpsertCartAsync(CartDto cartDto);
        Task<ResponseDto?> RemoveFromCartAsync(int cartDetailId);
        Task<ResponseDto?> ApplyCouponAsync(CartHeaderDto cartHeaderDto);
        Task<ResponseDto?> EmailCartAsync();
    }
}
