using PRN232.LMS.Course.API.Domain.Entities;
using PRN232.LMS.Course.API.Domain.Response;
using PRN232.LMS.Course.API.Domain.Entities;

namespace PRN232.LMS.Course.API.Application.Services;

public interface IEnrollmentService
{
    Task<PagedResultModel<EnrollmentBusinessModel>> GetEnrollmentsAsync(
        string? search,
        string? sort,
        int page,
        int size,
        List<string> expands,
        int? courseId = null);

    Task<EnrollmentBusinessModel?> GetEnrollmentByIdAsync(int id, List<string> expands);

    Task<EnrollmentBusinessModel> CreateEnrollmentAsync(EnrollmentBusinessModel enrollment);
    Task<EnrollmentBusinessModel?> UpdateEnrollmentAsync(int id, EnrollmentBusinessModel enrollment);
    Task<bool> DeleteEnrollmentAsync(int id);
}
