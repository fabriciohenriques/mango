namespace Mango.Services.ShoppingCartAPI.Models.Dto
{
    public class CartDto
    {
        public required CartHeaderDto CartHeader { get; set; }
        public IEnumerable<CartDetailDto>? CartDetails { get; set; }
    }
}
