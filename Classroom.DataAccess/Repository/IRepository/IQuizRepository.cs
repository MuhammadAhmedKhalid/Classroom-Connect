using Classroom.Models;

namespace Classroom.DataAccess.Repository.IRepository
{
    public interface IQuizRepository : IRepository<Quiz>
    {
        void Update(Quiz quizFromDb, Quiz quiz);
    }
}
