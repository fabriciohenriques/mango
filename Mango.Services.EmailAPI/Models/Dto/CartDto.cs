namespace Mango.Services.EmailAPI.Models.Dto
{
    public class CartDto
    {
        public required CartHeaderDto CartHeader { get; set; }
        public IEnumerable<CartDetailDto>? CartDetails { get; set; }
        public string Email { get; set; }
    }
}
