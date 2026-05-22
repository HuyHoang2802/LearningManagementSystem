using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.IServices;

public interface IEnrollmentService
{
    Task<PagedResultModel<EnrollmentBusinessModel>> GetEnrollmentsAsync(
        string? search,
        string? sort,
        int page,
        int size,
        List<string> expands);

    Task<EnrollmentBusinessModel?> GetEnrollmentByIdAsync(int id, List<string> expands);

    Task<EnrollmentBusinessModel> CreateEnrollmentAsync(EnrollmentBusinessModel enrollment);
}
