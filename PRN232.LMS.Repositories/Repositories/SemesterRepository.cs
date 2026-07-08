using PRN232.LMS.Models.Entities;
using PRN232.LMS.Repositories.IRepositories;

namespace PRN232.LMS.Repositories.Repositories
{
    public class SemesterRepository : GenericRepository<Semester>, ISemesterRepository
    {
        public SemesterRepository(LmsdbContext context) : base(context)
        {
        }
    }
}
