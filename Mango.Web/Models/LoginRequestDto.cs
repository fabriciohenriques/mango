using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class LoginRequestDto
    {
        [Required, EmailAddress]
        public required string UserName { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
