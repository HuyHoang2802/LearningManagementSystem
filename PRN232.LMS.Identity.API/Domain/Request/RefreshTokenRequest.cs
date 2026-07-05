using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Identity.API.Domain.Request
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
