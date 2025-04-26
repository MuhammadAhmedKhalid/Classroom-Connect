namespace Classroom.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IClassRepository Classes { get; }

        void Save();
    }
}
