using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Models.Entities;
using PRN232.LMS.Models.Request;
using PRN232.LMS.Models.Response;
using PRN232.LMS.Repositories.IRepositories;
using PRN232.LMS.Services.IServices;
using PRN232.LMS.Services.Utility;

namespace PRN232.LMS.Services.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<object>> GetStudentsAsync(StudentQueryRequest query)
        {
            var studentQuery = _unitOfWork.Students.GetQueryable();
            studentQuery = StudentQueryExtensions.Search(studentQuery, query);
            studentQuery = StudentQueryExtensions.Sort(studentQuery, query);

            var totalItems = await studentQuery.CountAsync();

            var expandEnrollments = query.Expand?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Any(x => x.Equals("enrollments", StringComparison.OrdinalIgnoreCase)) == true;

            studentQuery = StudentQueryExtensions.Expand(studentQuery, query);

            var safeSize = query.Size <= 0 ? 10 : query.Size;
            var safePage = query.Page <= 0 ? 1 : query.Page;

            studentQuery = StudentQueryExtensions.Paging(studentQuery, query);

            var studentList = await studentQuery.ToListAsync();

            var students = studentList.Select(s => new StudentResponse
            {
                StudentId = s.Studentid,
                FullName = s.Fullname,
                Email = s.Email,
                DateOfBirth = s.Dateofbirth,
                Enrollments = expandEnrollments && s.Enrollments != null
                    ? s.Enrollments.Select(e => new StudentEnrollmentResponse
                    {
                        EnrollmentId = e.Enrollmentid,
                        EnrollDate = e.Enrolldate ?? DateTime.MinValue,
                        Status = e.Status ?? string.Empty,
                        CourseId = e.Courseid,
                        CourseName = e.Course?.Coursename
                    }).ToList()
                    : null
            }).ToList();

            return new ApiResponse<object>
            {
                Success = true,
                Message = "Get students successfully",
                Data = students,
                Pagination = new PagedResponse
                {
                    Page = safePage,
                    PageSize = safeSize,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling((double)totalItems / safeSize)
                }
            };
        }

        public async Task<ApiResponse<object>> GetStudentByIdAsync(int id)
        {
            var studentQuery = _unitOfWork.Students.GetQueryable()
                .Where(x => x.Studentid == id)
                .Include(x => x.Enrollments)
                .ThenInclude(x => x.Course);

            var student = await studentQuery.FirstOrDefaultAsync();

            if (student == null)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Student not found"
                };
            }

            var studentResponse = new StudentResponse
            {
                StudentId = student.Studentid,
                FullName = student.Fullname,
                Email = student.Email,
                DateOfBirth = student.Dateofbirth,
                Enrollments = student.Enrollments?.Count > 0
                    ? student.Enrollments.Select(e => new StudentEnrollmentResponse
                    {
                        EnrollmentId = e.Enrollmentid,
                        EnrollDate = e.Enrolldate ?? DateTime.MinValue,
                        Status = e.Status ?? string.Empty,
                        CourseId = e.Courseid,
                        CourseName = e.Course?.Coursename
                    }).ToList()
                    : null
            };

            return new ApiResponse<object>
            {
                Success = true,
                Message = "Get student successfully",
                Data = studentResponse
            };
        }

        public async Task<ApiResponse<StudentResponse>> CreateStudentAsync(CreateStudentRequest request)
        {
            var newStudent = new Student
            {
                Fullname = request.FullName,
                Email = request.Email,
                Dateofbirth = DateOnly.FromDateTime(request.DateOfBirth)
            };

            await _unitOfWork.Students.AddAsync(newStudent);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<StudentResponse>
            {
                Success = true,
                Message = "Create student successfully",
                Data = new StudentResponse
                {
                    StudentId = newStudent.Studentid,
                    FullName = newStudent.Fullname,
                    Email = newStudent.Email
                }
            };
        }

        public async Task<ApiResponse<StudentResponse>> UpdateStudentAsync(int id, UpdateStudentRequest request)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
            {
                return new ApiResponse<StudentResponse>
                {
                    Success = false,
                    Message = "Student not found"
                };
            }

            student.Email = request.Email;
            student.Fullname = request.FullName;
            student.Dateofbirth = DateOnly.FromDateTime(request.DateOfBirth);

            await _unitOfWork.Students.UpdateAsync(student);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<StudentResponse>
            {
                Success = true,
                Message = "Update student successfully",
                Data = new StudentResponse
                {
                    StudentId = student.Studentid,
                    FullName = student.Fullname,
                    Email = student.Email
                }
            };
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student != null)
            {
                await _unitOfWork.Students.DeleteAsync(student.Studentid);
                await _unitOfWork.SaveChangesAsync();

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Delete student successfully",
                    Data = true
                };
            }

            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Delete student failed",
                Data = false
            };
        }
    }
}