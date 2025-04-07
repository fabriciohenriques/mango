using System.ComponentModel.DataAnnotations;

namespace Mango.Services.EmailAPI.Models
{
    public class EmailLogger
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(256)]
        public required string Email { get; set; }
        [Required]
        public required string Message { get; set; }
        public DateTime? EmailSent { get; set; }
    }
}
