using PRN232.LMS.Student.API.Domain.Entities;
using PRN232.LMS.Student.API.Domain.Response;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Student.API.Application.Services;

public interface ISemesterService
{
    Task<PagedResultModel<SemesterBusinessModel>> GetSemestersAsync(
        string? search,
        string? sort,
        int page,
        int size);

    Task<SemesterBusinessModel?> GetSemesterByIdAsync(int id);

    Task<SemesterBusinessModel> CreateSemesterAsync(SemesterBusinessModel semester);
    Task<SemesterBusinessModel?> UpdateSemesterAsync(int id, SemesterBusinessModel semester);
    Task<bool> DeleteSemesterAsync(int id);
}
