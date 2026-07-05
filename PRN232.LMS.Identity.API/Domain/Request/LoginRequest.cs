using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Identity.API.Domain.Request
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
