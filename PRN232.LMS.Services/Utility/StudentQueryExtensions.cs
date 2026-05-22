using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Models.Entities;
using PRN232.LMS.Models.Request;
using System.Linq;

namespace PRN232.LMS.Services.Utility
{
    public static class StudentQueryExtensions
    {
        public static IQueryable<Student> Search(this IQueryable<Student> query, StudentQueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Search))
            {
                return query;
            }

            return query.Where(x => x.Fullname.Contains(request.Search));
        }

        public static IQueryable<Student> Sort(this IQueryable<Student> query, StudentQueryRequest request)
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

        public static IQueryable<Student> Paging(this IQueryable<Student> query, StudentQueryRequest request)
        {
            var page = request.Page <= 0 ? 1 : request.Page;
            var size = request.Size <= 0 ? 10 : request.Size;

            return query.Skip((page - 1) * size).Take(size);
        }

        public static IQueryable<Student> Expand(this IQueryable<Student> query, StudentQueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Expand))
            {
                return query;
            }

            var expands = request.Expand.Split(
                ",",
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var item in expands)
            {
                switch (item.ToLower())
                {
                    case "enrollments":
                        query = query.Include(x => x.Enrollments)
                            .ThenInclude(x => x.Course);
                        break;
                }
            }

            return query;
        }
    }
}
