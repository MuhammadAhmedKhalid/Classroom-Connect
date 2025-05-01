using Classroom.Models;

namespace Classroom.DataAccess.Repository.IRepository
{
    public interface IAssignmentRepository : IRepository<Assignment>
    {
        void Update(Assignment assignmentFromDb, Assignment assignment);
    }
}
