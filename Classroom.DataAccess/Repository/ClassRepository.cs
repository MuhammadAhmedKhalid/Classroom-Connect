using Classroom.DataAccess.Data;
using Classroom.DataAccess.Repository.IRepository;
using Classroom.Models;

namespace Classroom.DataAccess.Repository
{
    public class ClassRepository : Repository<Class>, IClassRepository
    {

        private readonly ApplicationDbContext _db;

        public ClassRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Class classFromDb, Class @class)
        {
            classFromDb.Name = @class.Name;
            classFromDb.Description = @class.Description ?? string.Empty;

            _db.SaveChanges();
        }
    }
}
