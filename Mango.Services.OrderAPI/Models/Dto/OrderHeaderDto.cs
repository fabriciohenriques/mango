namespace Mango.Services.OrderAPI.Models.Dto
{
    public class OrderHeaderDto
    {
        public int OrderHeaderId { get; set; }
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
