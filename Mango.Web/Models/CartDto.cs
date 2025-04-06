namespace Mango.Web.Models
{
    public class CartDto
    {
        public required CartHeaderDto CartHeader { get; set; }
        public IEnumerable<CartDetailDto>? CartDetails { get; set; }
    }
}
