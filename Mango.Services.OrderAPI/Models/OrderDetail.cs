using Mango.Services.OrderAPI.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.OrderAPI.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailsId { get; set; }
        public int OrderHeaderId { get; set; }
        [Required, ForeignKey("OrderHeaderId")]
        public required OrderHeader OrderHeader { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public ProductDto? Product { get; set; }
        public int Count { get; set; }
        [Required, StringLength(50)]
        public required string ProductName { get; set; }
        public double Price { get; set; }
    }
}
