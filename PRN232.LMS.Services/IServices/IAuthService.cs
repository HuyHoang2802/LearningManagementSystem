using PRN232.LMS.Models.Request;
using PRN232.LMS.Models.Response;

namespace PRN232.LMS.Services.IServices
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    }
}
