using Classroom.Models;

namespace Classroom.DataAccess.Repository.IRepository
{
    public interface IAnnouncementRepository : IRepository<Announcement>
    {
        void Update(Announcement announcementFromDb, Announcement announcement);
    }
}
