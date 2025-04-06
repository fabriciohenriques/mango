using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class RegistrationRequestDto
    {
        [Required, EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string PhoneNumber { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
