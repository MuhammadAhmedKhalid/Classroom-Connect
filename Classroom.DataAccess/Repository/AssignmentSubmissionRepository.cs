using Classroom.DataAccess.Data;
using Classroom.DataAccess.Repository.IRepository;
using Classroom.Models;

namespace Classroom.DataAccess.Repository
{
    public class AssignmentSubmissionRepository : Repository<AssignmentSubmission>, IAssignmentSubmissionRepository
    {

        private readonly ApplicationDbContext _db;

        public AssignmentSubmissionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //public void Update(AssignmentSubmission assignmentSubmission)
        //{

        //}

    }
}
