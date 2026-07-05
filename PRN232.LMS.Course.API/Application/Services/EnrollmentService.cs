using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Course.API.Domain.Entities;
using PRN232.LMS.Course.API.Infrastructure.Repositories;
using PRN232.LMS.Course.API.Application.Services;
using PRN232.LMS.Course.API.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232.LMS.Course.API.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private const int DefaultPage = 1;
        private const int DefaultPageSize = 10;
        private const int MaxPageSize = 100;

        private readonly IUnitOfWork _unitOfWork;
        private readonly StudentGrpc.StudentGrpcClient _studentGrpcClient;

        public EnrollmentService(IUnitOfWork unitOfWork, StudentGrpc.StudentGrpcClient studentGrpcClient)
        {
            _unitOfWork = unitOfWork;
            _studentGrpcClient = studentGrpcClient;
        }

        public async Task<PagedResultModel<EnrollmentBusinessModel>> GetEnrollmentsAsync(
            string? search,
            string? sort,
            int page,
            int size,
            List<string> expands,
            int? courseId = null)
        {
            var normalizedPage = page <= 0 ? DefaultPage : page;
            var normalizedSize = size <= 0 ? DefaultPageSize : Math.Min(size, MaxPageSize);
            var normalizedExpands = NormalizeExpands(expands);
            var (sortBy, sortDescending) = NormalizeSort(sort);

            var query = _unitOfWork.Enrollments.GetQueryable();
            if (courseId.HasValue)
            {
                query = query.Where(enrollment => enrollment.Courseid == courseId.Value);
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

            var mappedItems = new List<EnrollmentBusinessModel>();
            foreach (var enrollment in enrollments)
            {
                var model = MapToBusinessModel(enrollment, normalizedExpands);
                if (normalizedExpands.Contains("student"))
                {
                    try
                    {
                        var grpcResponse = await _studentGrpcClient.GetStudentByIdAsync(new StudentRequest { StudentId = enrollment.Studentid });
                        if (grpcResponse.Exists)
                        {
                            model.Student = new StudentBusinessModel
                            {
                                StudentId = enrollment.Studentid,
                                FullName = grpcResponse.Fullname,
                                Email = grpcResponse.Email
                            };
                        }
                    }
                    catch
                    {
                        // Fallback in case Student Service is down
                    }
                }
                mappedItems.Add(model);
            }

            return new PagedResultModel<EnrollmentBusinessModel>
            {
                Items = mappedItems,
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
            if (normalizedExpands.Contains("course"))
            {
                query = query.Include(enrollment => enrollment.Course);
            }

            var enrollment = await query.FirstOrDefaultAsync(item => item.Enrollmentid == id);
            if (enrollment is null) return null;

            var model = MapToBusinessModel(enrollment, normalizedExpands);
            if (normalizedExpands.Contains("student"))
            {
                try
                {
                    var grpcResponse = await _studentGrpcClient.GetStudentByIdAsync(new StudentRequest { StudentId = enrollment.Studentid });
                    if (grpcResponse.Exists)
                    {
                        model.Student = new StudentBusinessModel
                        {
                            StudentId = enrollment.Studentid,
                            FullName = grpcResponse.Fullname,
                            Email = grpcResponse.Email
                        };
                    }
                }
                catch
                {
                    // Fallback
                }
            }

            return model;
        }

        public async Task<EnrollmentBusinessModel> CreateEnrollmentAsync(EnrollmentBusinessModel enrollment)
        {
            // Verify student existence using gRPC
            var grpcResponse = await _studentGrpcClient.CheckStudentExistsAsync(new StudentRequest { StudentId = enrollment.StudentId });
            if (!grpcResponse.Exists)
            {
                throw new ArgumentException("Student does not exist.");
            }

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

        public async Task<EnrollmentBusinessModel?> UpdateEnrollmentAsync(int id, EnrollmentBusinessModel enrollment)
        {
            var entity = await _unitOfWork.Enrollments.GetQueryable().FirstOrDefaultAsync(e => e.Enrollmentid == id);
            if (entity == null) return null;

            entity.Studentid = enrollment.StudentId;
            entity.Courseid = enrollment.CourseId;
            entity.Enrolldate = enrollment.EnrollDate;
            entity.Status = enrollment.Status;

            await _unitOfWork.Enrollments.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return MapToBusinessModel(entity, new HashSet<string>());
        }

        public async Task<bool> DeleteEnrollmentAsync(int id)
        {
            var entity = await _unitOfWork.Enrollments.GetQueryable().FirstOrDefaultAsync(e => e.Enrollmentid == id);
            if (entity == null) return false;

            await _unitOfWork.Enrollments.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return true;
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
                Student = null,
                Course = expands.Contains("course") && enrollment.Course is not null
                    ? MapCourse(enrollment.Course)
                    : null
            };
        }

        private static CourseBusinessModel MapCourse(PRN232.LMS.Course.API.Domain.Entities.Course course)
        {
            return new CourseBusinessModel
            {
                CourseId = course.Courseid,
                CourseName = course.Coursename,
                SemesterId = course.Semesterid
            };
        }
    }
}