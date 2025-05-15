using Classroom.DataAccess.Data;
using Classroom.DataAccess.Repository.IRepository;
using Classroom.Models;

namespace Classroom.DataAccess.Repository
{
    public class AnnouncementRepository : Repository<Announcement>, IAnnouncementRepository
    {
        private readonly ApplicationDbContext _db;

        public AnnouncementRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //public void Update()
        //{
        //}
    }
}
