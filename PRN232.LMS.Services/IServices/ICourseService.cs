using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.IServices;

public interface ICourseService
{
    Task<PagedResultModel<CourseBusinessModel>> GetCoursesAsync(
        string? search,
        string? sort,
        int page,
        int size,
        List<string> expands);

    Task<CourseBusinessModel?> GetCourseByIdAsync(int id, List<string> expands);

    Task<CourseBusinessModel?> UpdateCourseAsync(int id, CourseBusinessModel course);
    Task<bool> DeleteCourseAsync(int id);
     
    Task<CourseBusinessModel> CreateCourseAsync(CourseBusinessModel course);
}
