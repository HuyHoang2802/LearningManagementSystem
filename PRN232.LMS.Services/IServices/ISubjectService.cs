using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.IServices;

public interface ISubjectService
{
    Task<PagedResultModel<SubjectBusinessModel>> GetSubjectsAsync(
        string? search,
        string? sort,
        int page,
        int size);

    Task<SubjectBusinessModel?> GetSubjectByIdAsync(int id);

    Task<SubjectBusinessModel> CreateSubjectAsync(SubjectBusinessModel subject);
    Task<SubjectBusinessModel?> UpdateSubjectAsync(int id, SubjectBusinessModel subject);
    Task<bool> DeleteSubjectAsync(int id);
}
