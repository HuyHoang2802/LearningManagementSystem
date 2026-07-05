using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Student.API.Domain.Request;
using System.Linq;

namespace PRN232.LMS.Student.API.Helpers
{
    public static class StudentQueryExtensions
    {
        public static IQueryable<PRN232.LMS.Student.API.Domain.Entities.Student> Search(
            this IQueryable<PRN232.LMS.Student.API.Domain.Entities.Student> query, 
            StudentQueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Search))
            {
                return query;
            }

            return query.Where(x => x.Fullname.Contains(request.Search));
        }

        public static IQueryable<PRN232.LMS.Student.API.Domain.Entities.Student> Sort(
            this IQueryable<PRN232.LMS.Student.API.Domain.Entities.Student> query, 
            StudentQueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Sort))
            {
                return query.OrderByDescending(x => x.Studentid);
            }

            return request.Sort switch
            {
                "fullName" => query.OrderBy(x => x.Fullname),
                "-fullName" => query.OrderByDescending(x => x.Fullname),
                "dateOfBirth" => query.OrderBy(x => x.Dateofbirth),
                "-dateOfBirth" => query.OrderByDescending(x => x.Dateofbirth),
                _ => query.OrderByDescending(x => x.Studentid)
            };
        }

        public static IQueryable<PRN232.LMS.Student.API.Domain.Entities.Student> Paging(
            this IQueryable<PRN232.LMS.Student.API.Domain.Entities.Student> query, 
            StudentQueryRequest request)
        {
            var page = request.Page <= 0 ? 1 : request.Page;
            var size = request.Size <= 0 ? 10 : request.Size;

            return query.Skip((page - 1) * size).Take(size);
        }

        public static IQueryable<PRN232.LMS.Student.API.Domain.Entities.Student> Expand(
            this IQueryable<PRN232.LMS.Student.API.Domain.Entities.Student> query, 
            StudentQueryRequest request)
        {
            return query;
        }
    }
}