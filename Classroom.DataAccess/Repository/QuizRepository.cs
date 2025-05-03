using Classroom.DataAccess.Data;
using Classroom.DataAccess.Repository.IRepository;
using Classroom.Models;

namespace Classroom.DataAccess.Repository
{
    public class QuizRepository : Repository<Quiz>, IQuizRepository
    {

        private readonly ApplicationDbContext _db;

        public QuizRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Quiz quizFromDb, Quiz quiz)
        {
            quizFromDb.Title = quiz.Title;
            quizFromDb.Instructions = quiz.Instructions ?? string.Empty;
            quizFromDb.DueDate = quiz.DueDate;
            quizFromDb.CloseDate = quiz.CloseDate;
        }

    }
}
