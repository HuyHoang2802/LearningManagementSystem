using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Models.Entities;

namespace PRN232.LMS.Services.Utility
{
    public static class StudentQueryExtensions
    {
        public static IQueryable<Student> Search(this IQueryable<Student> query, string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return query;
            }

            return query.Where(x => EF.Functions.ILike(x.Fullname, $"%{search.Trim()}%"));
        }

        public static IQueryable<Student> Sort(this IQueryable<Student> query, string? sort)
        {
            if (string.IsNullOrWhiteSpace(sort))
            {
                return query.OrderByDescending(x => x.Studentid);
            }

            var trimmedSort = sort.Trim();
            var sortDescending = trimmedSort.StartsWith('-');
            var sortBy = sortDescending ? trimmedSort[1..] : trimmedSort;

            return sortBy switch
            {
                "fullName" => sortDescending ? query.OrderByDescending(x => x.Fullname) : query.OrderBy(x => x.Fullname),
                "dateOfBirth" => sortDescending ? query.OrderByDescending(x => x.Dateofbirth) : query.OrderBy(x => x.Dateofbirth),
                _ => query.OrderByDescending(x => x.Studentid)
            };
        }

        public static IQueryable<Student> Paging(this IQueryable<Student> query, int page, int size)
        {
            var safePage = page <= 0 ? 1 : page;
            var safeSize = size <= 0 ? 10 : size;

            return query.Skip((safePage - 1) * safeSize).Take(safeSize);
        }

        public static IQueryable<Student> Expand(this IQueryable<Student> query, IEnumerable<string> expands)
        {
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
