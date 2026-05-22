using Azure.Core;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Models.Entities;
using PRN232.LMS.Models.Request;
using PRN232.LMS.Models.Response;
using PRN232.LMS.Repositories.IRepositories;
using PRN232.LMS.Repositories.Repositories;
using PRN232.LMS.Services.IServices;
using PRN232.LMS.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PRN232.LMS.Services.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<StudentResponse>>> GetStudentsAsync(StudentQueryRequest query)
        {
            var studentQuery = _unitOfWork.Students.GetQueryable();
            studentQuery = StudentQueryExtensions.Search(studentQuery, query);
            // Sort
            studentQuery = StudentQueryExtensions.Sort(studentQuery, query);

            // TOTAL ITEMS
            var totalItems = await studentQuery.CountAsync();

            // Pading
            studentQuery = StudentQueryExtensions.Paging(studentQuery, query);

            var studentList = await studentQuery.ToListAsync();


            var students = studentList.Select(s => new StudentResponse
            {
                StudentId = s.Studentid,

                FullName = s.Fullname,

                Email = s.Email,
                DateOfBirth = s.Dateofbirth,

            }).ToList();





            return new ApiResponse<List<StudentResponse>>
            {
                success = true,
                message = "Get students successfully",
                Data = students,
                pagination = new PagedResponse
                {
                    Page = query.Page,
                    PageSize = query.Size,
                    TotalItems = totalItems,
                    TotalPages =
                            (int)Math.Ceiling(
                                (double)totalItems
                                / query.Size)
                }
            };
        }
    



        public async Task<StudentResponse?> GetStudentByIdAsync(int id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);

            if (student == null)
            {
               
                return null;
            }

    
            var toStudentResponse = new StudentResponse
            {
                StudentId = student.Studentid,
                FullName = student.Fullname,
                Email = student.Email
            };

            return toStudentResponse;
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
                success = true,

                message = "Create student successfully",

                Data = new StudentResponse
                {
                    StudentId = newStudent.Studentid,
                    FullName = newStudent.Fullname,
                    Email = newStudent.Email
                }
            }
            ;
        }
        public async Task<ApiResponse<StudentResponse>> UpdateStudentAsync(int id, UpdateStudentRequest request)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
            {
                return new ApiResponse<StudentResponse>
                {
                    success = false,
                    message = "Student not found"
                };
            }

            // Cập nhật thông tin
            student.Email = request.Email;
            student.Fullname = request.FullName;
            student.Dateofbirth = DateOnly.FromDateTime(request.DateOfBirth);

            _unitOfWork.Students.UpdateAsync(student);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<StudentResponse>
            {
                success = true,
                message = "Update student successfully",
                // Map thủ công sang StudentResponse thay vì dùng ToStudentResponse()
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
                    success = true,
                    message = "Delete student successfully"
                };
            }

            return new ApiResponse<bool>
            {
                success = false,
                message = "Delete student Fails"
            };
        }

        
    }
}