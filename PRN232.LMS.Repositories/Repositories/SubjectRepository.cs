using PRN232.LMS.Models.Entities;
using PRN232.LMS.Repositories.IRepositories;

namespace PRN232.LMS.Repositories.Repositories
{
    public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(LmsdbContext context) : base(context)
        {
        }
    }
}
