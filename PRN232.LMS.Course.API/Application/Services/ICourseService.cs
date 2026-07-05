using PRN232.LMS.Course.API.Domain.Entities;
using PRN232.LMS.Course.API.Domain.Response;
using PRN232.LMS.Course.API.Domain.Entities;

namespace PRN232.LMS.Course.API.Application.Services;

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
