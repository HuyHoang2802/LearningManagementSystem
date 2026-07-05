using PRN232.LMS.Course.API.Domain.Entities;
using PRN232.LMS.Course.API.Domain.Response;
using System;
using System.Collections.Generic;

namespace PRN232.LMS.Course.API.Helpers
{
    public static class EnrollmentMappingHelper
    {
        public static readonly IReadOnlyDictionary<string, Func<EnrollmentResponseModel, object?>> FieldSelectors =
            new Dictionary<string, Func<EnrollmentResponseModel, object?>>(StringComparer.OrdinalIgnoreCase)
            {
                ["enrollmentId"] = enrollment => enrollment.EnrollmentId,
                ["studentId"] = enrollment => enrollment.StudentId,
                ["courseId"] = enrollment => enrollment.CourseId,
                ["enrollDate"] = enrollment => enrollment.EnrollDate,
                ["status"] = enrollment => enrollment.Status,
                ["student"] = enrollment => enrollment.Student,
                ["course"] = enrollment => enrollment.Course
            };

        public static EnrollmentResponseModel MapToResponseModel(EnrollmentBusinessModel enrollment)
        {
            return new EnrollmentResponseModel
            {
                EnrollmentId = enrollment.EnrollmentId,
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                EnrollDate = enrollment.EnrollDate,
                Status = enrollment.Status,
                Student = enrollment.Student is null ? null : MapStudent(enrollment.Student),
                Course = enrollment.Course is null ? null : MapCourse(enrollment.Course)
            };
        }

        private static StudentResponseModel MapStudent(StudentBusinessModel student)
        {
            return new StudentResponseModel
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth
            };
        }

        private static CourseResponseModel MapCourse(CourseBusinessModel course)
        {
            return new CourseResponseModel
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                SemesterId = course.SemesterId
            };
        }
    }
}
