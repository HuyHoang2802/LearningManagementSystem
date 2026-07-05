using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Student.API.Domain.Entities;
using PRN232.LMS.Student.API.Domain.Request;
using PRN232.LMS.Student.API.Domain.Response;
using PRN232.LMS.Student.API.Infrastructure.Repositories;
using PRN232.LMS.Student.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232.LMS.Student.API.Application.Services
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
                Enrollments = null
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
            var student = await _unitOfWork.Students.GetByIdAsync(id);

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
                Enrollments = null
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
            var newStudent = new PRN232.LMS.Student.API.Domain.Entities.Student
            {
                StudentCode = request.StudentCode,
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
                    Email = newStudent.Email,
                    DateOfBirth = newStudent.Dateofbirth
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

            student.StudentCode = request.StudentCode;
            student.Fullname = request.FullName;
            student.Email = request.Email;
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
                    Email = student.Email,
                    DateOfBirth = student.Dateofbirth
                }
            };
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Student not found",
                    Data = false
                };
            }

            await _unitOfWork.Students.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Delete student successfully",
                Data = true
            };
        }
    }
}