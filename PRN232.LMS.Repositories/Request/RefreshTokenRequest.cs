using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Models.Request
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
