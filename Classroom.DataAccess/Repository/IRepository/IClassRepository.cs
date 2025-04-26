using Classroom.Models;

namespace Classroom.DataAccess.Repository.IRepository
{
    public interface IClassRepository : IRepository<Class>
    {
        void Update(Class classFromDb, Class @class);
    }
}
