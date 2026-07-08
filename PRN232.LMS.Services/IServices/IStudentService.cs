using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.IServices;

public interface IStudentService
{
    Task<PagedResultModel<StudentBusinessModel>> GetStudentsAsync(
        string? search,
        string? sort,
        int page,
        int size,
        List<string> expands);

    Task<StudentBusinessModel?> GetStudentByIdAsync(int id, List<string> expands);

    Task<StudentBusinessModel> CreateStudentAsync(StudentBusinessModel student);

    Task<StudentBusinessModel?> UpdateStudentAsync(int id, StudentBusinessModel student);

    Task<bool> DeleteAsync(int id);
}

