using PRN232.LMS.Identity.API.Domain.Request;
using PRN232.LMS.Identity.API.Domain.Response;

namespace PRN232.LMS.Identity.API.Application.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    }
}
