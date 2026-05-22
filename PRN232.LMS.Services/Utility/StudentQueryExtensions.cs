using PRN232.LMS.Models.Entities;
using PRN232.LMS.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Utility
{
    public static class StudentQueryExtensions
    {
        public static IQueryable<Student> Search(this IQueryable<Student> query, StudentQueryRequest request)
        {
            if (string.IsNullOrEmpty(request.Search))
            {
                return query;
            }
            var search = request.Search.ToLower();

            return query.Where(x => x.Fullname.Contains(search));
        }


        public static IQueryable<Student> Sort(this IQueryable<Student> query, StudentQueryRequest request)
        {
            return request.Sort switch
            {
                "fullName" => query.OrderBy(
                        x => x.Fullname),

                "-fullName" => query.OrderByDescending(
                            x => x.Fullname),
                "dateOfBirth" => query.OrderBy(x => x.Dateofbirth),

                "-dateOfBirth" => query.OrderByDescending(x => x.Dateofbirth),
                _ => query.OrderBy(x => x.Studentid)
            };
        }

        public static IQueryable<Student> Paging(
            this IQueryable<Student> query,
            StudentQueryRequest request)
        {
            return query.Skip((request.Page - 1) * request.Size).Take(request.Size);
        }
    }
}
