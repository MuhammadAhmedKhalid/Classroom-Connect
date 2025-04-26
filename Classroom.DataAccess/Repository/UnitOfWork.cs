using Classroom.DataAccess.Data;
using Classroom.DataAccess.Repository.IRepository;

namespace Classroom.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public IClassRepository Classes { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Classes = new ClassRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
