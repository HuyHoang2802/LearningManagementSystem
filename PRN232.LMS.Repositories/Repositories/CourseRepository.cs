using PRN232.LMS.Models.Entities;
using PRN232.LMS.Repositories.IRepositories;

namespace PRN232.LMS.Repositories.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(LmsdbContext context) : base(context)
        {
        }
    }
}
