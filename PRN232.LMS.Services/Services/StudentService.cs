using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Models.Entities;
using PRN232.LMS.Repositories.IRepositories;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.IServices;
using PRN232.LMS.Services.Utility;

namespace PRN232.LMS.Services.Services;

public class StudentService : IStudentService
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    private readonly IUnitOfWork _unitOfWork;

    public StudentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResultModel<StudentBusinessModel>> GetStudentsAsync(
        string? search,
        string? sort,
        int page,
        int size,
        List<string> expands)
    {
        var normalizedPage = page <= 0 ? DefaultPage : page;
        var normalizedSize = size <= 0 ? DefaultPageSize : Math.Min(size, MaxPageSize);
        var normalizedExpands = NormalizeExpands(expands);

        var query = _unitOfWork.Students.GetQueryable();
        query = StudentQueryExtensions.Expand(query, normalizedExpands);
        query = StudentQueryExtensions.Search(query, search);

        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)normalizedSize);

        var students = await StudentQueryExtensions.Sort(query, sort)
            .Skip((normalizedPage - 1) * normalizedSize)
            .Take(normalizedSize)
            .ToListAsync();

        return new PagedResultModel<StudentBusinessModel>
        {
            Items = students.Select(s => MapToBusinessModel(s, normalizedExpands)).ToList(),
            Page = normalizedPage,
            PageSize = normalizedSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task<StudentBusinessModel?> GetStudentByIdAsync(int id, List<string> expands)
    {
        var normalizedExpands = NormalizeExpands(expands);
        var query = _unitOfWork.Students.GetQueryable();
        query = StudentQueryExtensions.Expand(query, normalizedExpands);

        var student = await query.FirstOrDefaultAsync(s => s.Studentid == id);
        return student is null ? null : MapToBusinessModel(student, normalizedExpands);
    }

    public async Task<StudentBusinessModel> CreateStudentAsync(StudentBusinessModel student)
    {
        var entity = new Student
        {
            Fullname = student.FullName,
            Email = student.Email,
            Dateofbirth = DateOnly.FromDateTime(student.DateOfBirth)
        };

        await _unitOfWork.Students.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return MapToBusinessModel(entity, new HashSet<string>());
    }

    public async Task<StudentBusinessModel?> UpdateStudentAsync(int id, StudentBusinessModel student)
    {
        var existing = await _unitOfWork.Students.GetByIdAsync(id);
        if (existing is null)
        {
            return null;
        }

        existing.Fullname = student.FullName;
        existing.Email = student.Email;
        existing.Dateofbirth = DateOnly.FromDateTime(student.DateOfBirth);

        await _unitOfWork.Students.UpdateAsync(existing);
        await _unitOfWork.SaveChangesAsync();

        return MapToBusinessModel(existing, new HashSet<string>());
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);
        if (student is null)
        {
            return false;
        }

        await _unitOfWork.Students.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private static ISet<string> NormalizeExpands(IEnumerable<string> expands)
    {
        return expands
            .Where(expand => !string.IsNullOrWhiteSpace(expand))
            .Select(expand => expand.Trim().ToLowerInvariant())
            .ToHashSet();
    }

    private static StudentBusinessModel MapToBusinessModel(Student student, ISet<string> expands)
    {
        return new StudentBusinessModel
        {
            StudentId = student.Studentid,
            FullName = student.Fullname,
            Email = student.Email,
            DateOfBirth = student.Dateofbirth?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
            Enrollments = expands.Contains("enrollments") && student.Enrollments is not null
                ? student.Enrollments.Select(MapEnrollment).ToList()
                : null
        };
    }

    private static EnrollmentBusinessModel MapEnrollment(Enrollment enrollment)
    {
        return new EnrollmentBusinessModel
        {
            EnrollmentId = enrollment.Enrollmentid,
            StudentId = enrollment.Studentid,
            CourseId = enrollment.Courseid,
            EnrollDate = enrollment.Enrolldate ?? DateTime.MinValue,
            Status = enrollment.Status ?? string.Empty,
            Course = enrollment.Course is null ? null : new CourseBusinessModel
            {
                CourseId = enrollment.Course.Courseid,
                CourseName = enrollment.Course.Coursename,
                SemesterId = enrollment.Course.Semesterid
            }
        };
    }
}