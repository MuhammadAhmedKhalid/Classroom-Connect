using Classroom.DataAccess.Data;
using Classroom.DataAccess.Repository.IRepository;
using Classroom.Models;

namespace Classroom.DataAccess.Repository
{
    public class QuizAnswerRepository : Repository<QuizAnswer>, IQuizAnswerRepository
    {

        private readonly ApplicationDbContext _db;

        public QuizAnswerRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //public void Update(QuizAnswer quizAnswer)
        //{

        //}

    }
}
