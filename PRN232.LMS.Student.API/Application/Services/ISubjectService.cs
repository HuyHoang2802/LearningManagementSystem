using PRN232.LMS.Student.API.Domain.Entities;
using PRN232.LMS.Student.API.Domain.Response;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Student.API.Application.Services;

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
