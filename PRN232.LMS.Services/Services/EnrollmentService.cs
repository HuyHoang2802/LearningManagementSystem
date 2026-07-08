using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Models.Entities;
using PRN232.LMS.Repositories.IRepositories;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.IServices;

namespace PRN232.LMS.Services.Services;

public class EnrollmentService : IEnrollmentService
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    private readonly IUnitOfWork _unitOfWork;

    public EnrollmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResultModel<EnrollmentBusinessModel>> GetEnrollmentsAsync(
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

        var query = _unitOfWork.Enrollments.GetQueryable();
        if (normalizedExpands.Contains("student"))
        {
            query = query.Include(enrollment => enrollment.Student);
        }
        if (normalizedExpands.Contains("course"))
        {
            query = query.Include(enrollment => enrollment.Course);
        }

        query = ApplySearch(query, search);
        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)normalizedSize);

        var enrollments = await ApplySort(query, sortBy, sortDescending)
            .Skip((normalizedPage - 1) * normalizedSize)
            .Take(normalizedSize)
            .ToListAsync();

        return new PagedResultModel<EnrollmentBusinessModel>
        {
            Items = enrollments.Select(enrollment => MapToBusinessModel(enrollment, normalizedExpands)).ToList(),
            Page = normalizedPage,
            PageSize = normalizedSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task<EnrollmentBusinessModel?> GetEnrollmentByIdAsync(int id, List<string> expands)
    {
        var normalizedExpands = NormalizeExpands(expands);
        var query = _unitOfWork.Enrollments.GetQueryable();
        if (normalizedExpands.Contains("student"))
        {
            query = query.Include(enrollment => enrollment.Student);
        }
        if (normalizedExpands.Contains("course"))
        {
            query = query.Include(enrollment => enrollment.Course);
        }

        var enrollment = await query.FirstOrDefaultAsync(item => item.Enrollmentid == id);
        return enrollment is null ? null : MapToBusinessModel(enrollment, normalizedExpands);
    }

    public async Task<EnrollmentBusinessModel> CreateEnrollmentAsync(EnrollmentBusinessModel enrollment)
    {
        var entity = new Enrollment
        {
            Studentid = enrollment.StudentId,
            Courseid = enrollment.CourseId,
            Enrolldate = enrollment.EnrollDate,
            Status = enrollment.Status
        };

        await _unitOfWork.Enrollments.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return MapToBusinessModel(entity, new HashSet<string>());
    }

    private static IQueryable<Enrollment> ApplySearch(IQueryable<Enrollment> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var keyword = search.Trim();
        return query.Where(enrollment => enrollment.Status != null && EF.Functions.ILike(enrollment.Status, $"%{keyword}%"));
    }

    private static IQueryable<Enrollment> ApplySort(
        IQueryable<Enrollment> query,
        string sortBy,
        bool sortDescending)
    {
        return sortBy switch
        {
            "status" => sortDescending
                ? query.OrderByDescending(enrollment => enrollment.Status)
                : query.OrderBy(enrollment => enrollment.Status),
            "enrollDate" => sortDescending
                ? query.OrderByDescending(enrollment => enrollment.Enrolldate)
                : query.OrderBy(enrollment => enrollment.Enrolldate),
            _ => sortDescending
                ? query.OrderByDescending(enrollment => enrollment.Enrollmentid)
                : query.OrderBy(enrollment => enrollment.Enrollmentid)
        };
    }

    private static (string SortBy, bool SortDescending) NormalizeSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return ("enrollmentId", false);
        }

        var trimmedSort = sort.Trim();
        var sortDescending = trimmedSort.StartsWith('-');
        var sortBy = sortDescending ? trimmedSort[1..] : trimmedSort;

        return sortBy switch
        {
            "enrollmentId" => (sortBy, sortDescending),
            "enrollDate" => (sortBy, sortDescending),
            "status" => (sortBy, sortDescending),
            _ => ("enrollmentId", false)
        };
    }

    private static ISet<string> NormalizeExpands(IEnumerable<string> expands)
    {
        return expands
            .Where(expand => !string.IsNullOrWhiteSpace(expand))
            .Select(expand => expand.Trim().ToLowerInvariant())
            .ToHashSet();
    }

    private static EnrollmentBusinessModel MapToBusinessModel(
        Enrollment enrollment,
        ISet<string> expands)
    {
        return new EnrollmentBusinessModel
        {
            EnrollmentId = enrollment.Enrollmentid,
            StudentId = enrollment.Studentid,
            CourseId = enrollment.Courseid,
            EnrollDate = enrollment.Enrolldate ?? DateTime.MinValue,
            Status = enrollment.Status ?? string.Empty,
            Student = expands.Contains("student") && enrollment.Student is not null
                ? MapStudent(enrollment.Student)
                : null,
            Course = expands.Contains("course") && enrollment.Course is not null
                ? MapCourse(enrollment.Course)
                : null
        };
    }

    private static StudentBusinessModel MapStudent(Student student)
    {
        return new StudentBusinessModel
        {
            StudentId = student.Studentid,
            FullName = student.Fullname,
            Email = student.Email,
            DateOfBirth = student.Dateofbirth?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue
        };
    }

    private static CourseBusinessModel MapCourse(Course course)
    {
        return new CourseBusinessModel
        {
            CourseId = course.Courseid,
            CourseName = course.Coursename,
            SemesterId = course.Semesterid
        };
    }
}
