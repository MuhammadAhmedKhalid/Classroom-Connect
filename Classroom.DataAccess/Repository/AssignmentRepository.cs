using Classroom.DataAccess.Data;
using Classroom.DataAccess.Repository.IRepository;
using Classroom.Models;

namespace Classroom.DataAccess.Repository
{
    public class AssignmentRepository : Repository<Assignment>, IAssignmentRepository
    {

        private readonly ApplicationDbContext _db;

        public AssignmentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //public void Update(Assignment assignment)
        //{
            
        //}
    }
}
