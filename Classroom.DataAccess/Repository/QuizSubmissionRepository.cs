using Classroom.DataAccess.Data;
using Classroom.DataAccess.Repository.IRepository;
using Classroom.Models;

namespace Classroom.DataAccess.Repository
{
    public class QuizSubmissionRepository : Repository<QuizSubmission>, IQuizSubmissionRepository
    {

        private readonly ApplicationDbContext _db;

        public QuizSubmissionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //public void Update(QuizSubmission quizSubmission)
        //{

        //}

    }
}
