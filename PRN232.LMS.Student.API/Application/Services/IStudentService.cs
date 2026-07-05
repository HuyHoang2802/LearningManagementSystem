using System.Collections.Generic;
using System.Threading.Tasks;
using PRN232.LMS.Student.API.Domain.Request;
using PRN232.LMS.Student.API.Domain.Response;
using PRN232.LMS.Student.API.Domain.Entities;


namespace PRN232.LMS.Student.API.Application.Services
{
    public interface IStudentService
    {
        Task<ApiResponse<object>> GetStudentsAsync(StudentQueryRequest query);
        Task<ApiResponse<object>> GetStudentByIdAsync(int id);
        Task<ApiResponse<StudentResponse>> UpdateStudentAsync(int id, UpdateStudentRequest request);
        Task<ApiResponse<StudentResponse>> CreateStudentAsync(CreateStudentRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
