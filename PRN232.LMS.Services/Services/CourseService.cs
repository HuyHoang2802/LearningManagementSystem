using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Models.Entities;
using PRN232.LMS.Repositories.IRepositories;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.IServices;

namespace PRN232.LMS.Services.Services;

public class CourseService : ICourseService
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    private readonly IUnitOfWork _unitOfWork;

    public CourseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResultModel<CourseBusinessModel>> GetCoursesAsync(
        string? search,
        string? sort,
        int page,
        int size,
        List<string> expands)
    {
        var normalizedPage = page <= 0 ? DefaultPage : page;
        var normalizedSize = size <= 0 ? DefaultPageSize : Math.Min(size, MaxPageSize);
        var normalizedExpands = NormalizeExpands(expands);
        var (sortBy, sortDescending) = NormalizeSort(sort);

        var query = _unitOfWork.Courses.GetQueryable();
        if (normalizedExpands.Contains("semester"))
        {
            query = query.Include(course => course.Semester);
        }

        query = ApplySearch(query, search);
        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)normalizedSize);

        var courses = await ApplySort(query, sortBy, sortDescending)
            .Skip((normalizedPage - 1) * normalizedSize)
            .Take(normalizedSize)
            .ToListAsync();

        return new PagedResultModel<CourseBusinessModel>
        {
            Items = courses.Select(course => MapToBusinessModel(course, normalizedExpands)).ToList(),
            Page = normalizedPage,
            PageSize = normalizedSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task<CourseBusinessModel?> GetCourseByIdAsync(int id, List<string> expands)
    {
        var normalizedExpands = NormalizeExpands(expands);
        var query = _unitOfWork.Courses.GetQueryable();
        if (normalizedExpands.Contains("semester"))
        {
            query = query.Include(course => course.Semester);
        }

        var course = await query.FirstOrDefaultAsync(item => item.Courseid == id);
        return course is null ? null : MapToBusinessModel(course, normalizedExpands);
    }

    public async Task<CourseBusinessModel> CreateCourseAsync(CourseBusinessModel course)
    {
        var entity = new Course
        {
            Coursename = course.CourseName,
            Semesterid = course.SemesterId
        };

        await _unitOfWork.Courses.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return MapToBusinessModel(entity, new HashSet<string>());
    }

    private static IQueryable<Course> ApplySearch(IQueryable<Course> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var keyword = search.Trim();
        return query.Where(course => EF.Functions.ILike(course.Coursename, $"%{keyword}%"));
    }

    private static IQueryable<Course> ApplySort(
        IQueryable<Course> query,
        string sortBy,
        bool sortDescending)
    {
        return sortBy switch
        {
            "semesterId" => sortDescending
                ? query.OrderByDescending(course => course.Semesterid)
                : query.OrderBy(course => course.Semesterid),
            _ => sortDescending
                ? query.OrderByDescending(course => course.Coursename)
                : query.OrderBy(course => course.Coursename)
        };
    }

    private static (string SortBy, bool SortDescending) NormalizeSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return ("courseName", false);
        }

        var trimmedSort = sort.Trim();
        var sortDescending = trimmedSort.StartsWith('-');
        var sortBy = sortDescending ? trimmedSort[1..] : trimmedSort;

        return sortBy switch
        {
            "courseName" => (sortBy, sortDescending),
            "semesterId" => (sortBy, sortDescending),
            _ => ("courseName", false)
        };
    }

    private static ISet<string> NormalizeExpands(IEnumerable<string> expands)
    {
        return expands
            .Where(expand => !string.IsNullOrWhiteSpace(expand))
            .Select(expand => expand.Trim().ToLowerInvariant())
            .ToHashSet();
    }

    private static CourseBusinessModel MapToBusinessModel(
        Course course,
        ISet<string> expands)
    {
        return new CourseBusinessModel
        {
            CourseId = course.Courseid,
            CourseName = course.Coursename,
            SemesterId = course.Semesterid,
            Semester = expands.Contains("semester") && course.Semester is not null
                ? MapSemester(course.Semester)
                : null
        };
    }

    private static SemesterBusinessModel MapSemester(Semester semester)
    {
        return new SemesterBusinessModel
        {
            SemesterId = semester.Semesterid,
            SemesterName = semester.Semestername,
            StartDate = semester.Startdate.ToDateTime(TimeOnly.MinValue),
            EndDate = semester.Enddate.ToDateTime(TimeOnly.MinValue)
        };
    }
}
