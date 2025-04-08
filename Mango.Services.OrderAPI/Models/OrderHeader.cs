using System.ComponentModel.DataAnnotations;

namespace Mango.Services.OrderAPI.Models
{
    public class OrderHeader
    {
        [Key]
        public int OrderHeaderId { get; set; }
        [StringLength(50)]
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double OrderTotal { get; set; }
        public Guid UserId { get; set; }
        public DateTime OrderTime { get; set; }
        public string? Status { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
