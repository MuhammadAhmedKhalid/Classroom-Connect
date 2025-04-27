using Classroom.DataAccess.Data;
using Classroom.DataAccess.Repository.IRepository;
using Classroom.Models;

namespace Classroom.DataAccess.Repository
{
    public class QuizQuestionRepository : Repository<QuizQuestion>, IQuizQuestionRepository
    {

        private readonly ApplicationDbContext _db;

        public QuizQuestionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //public void Update(QuizQuestion quizQuestion)
        //{

        //}

    }
}
