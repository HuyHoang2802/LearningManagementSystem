using System.Collections.Generic;
using System.Threading.Tasks;
using PRN232.LMS.Models.Request;
using PRN232.LMS.Models.Response;
using PRN232.LMS.Models.Entities;


namespace PRN232.LMS.Services.IServices
{
    public interface IStudentService
    {
        Task<ApiResponse<List<StudentResponse>>> GetStudentsAsync(StudentQueryRequest query);
        Task<StudentResponse?> GetStudentByIdAsync(int id);
        Task<ApiResponse<StudentResponse>> UpdateStudentAsync(int id, UpdateStudentRequest request);
        Task<ApiResponse<StudentResponse>> CreateStudentAsync(CreateStudentRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}