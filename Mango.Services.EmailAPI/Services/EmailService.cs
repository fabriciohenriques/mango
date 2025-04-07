using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public EmailService(DbContextOptions<AppDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task EmailCartAndLog(CartDto cartDto)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<h1>Cart Details</h1>");
            sb.AppendLine("<h2>Cart Header</h2>");
            sb.AppendLine($"<p>Total: {cartDto.CartHeader.CartTotal.ToString("C2")}</p>");
            sb.AppendLine("<br/>");
            sb.AppendLine("<ul>");
            foreach (var item in cartDto.CartDetails)
            {
                sb.AppendLine("<li>");
                sb.AppendLine($"<h3>{item.Product.Name}</h3>");
                sb.AppendLine($"<p>Quantity: {item.Count}</p>");
                sb.AppendLine($"<p>Price: {(item.Product.Price * item.Count).ToString("C2")}</p>");
                sb.AppendLine("</li>");
            }
            sb.AppendLine("</ul>");

            await LogAndEmail(sb.ToString(), cartDto.Email);
        }

        public async Task LogRegisteredUser(string email)
        {
            var message = $"New user registered with email: {email}";
            await LogAndEmail(message, email);
        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            var emailLog = new EmailLogger
            {
                Email = email,
                Message = message,
                EmailSent = DateTime.Now
            };

            try
            {
                await using var db = new AppDbContext(_dbOptions);
                await db.EmailLoggers.AddAsync(emailLog);
                await db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
