using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        [Required]
        public required string Name { get; set; }
        [Range(1, 1000)]
        public double Price { get; set; }
        [StringLength(200)]
        public string? Description { get; set; }
        [StringLength(50)]
        public string? CategoryName { get; set; }
        [StringLength(500)]
        public string? ImageUrl { get; set; }
    }
}
