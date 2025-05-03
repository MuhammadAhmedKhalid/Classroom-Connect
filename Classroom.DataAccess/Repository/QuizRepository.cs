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

            var existingQuestions = _db.QuizQuestions.Where(q => q.QuizId == quizFromDb.Id).ToList();

            foreach (var updatedQuestion in quiz.Questions)
            {
                if (updatedQuestion.Id == 0)
                {
                    // New question
                    quizFromDb.Questions.Add(new QuizQuestion
                    {
                        Text = updatedQuestion.Text,
                        CorrectAnswer = updatedQuestion.CorrectAnswer,
                        QuizId = quizFromDb.Id
                    });
                }
                else
                {
                    // Update existing question
                    var existing = existingQuestions.FirstOrDefault(q => q.Id == updatedQuestion.Id);
                    if (existing != null)
                    {
                        existing.Text = updatedQuestion.Text;
                        existing.CorrectAnswer = updatedQuestion.CorrectAnswer;
                    }
                }
            }

            // Remove deleted questions
            foreach (var existing in existingQuestions)
            {
                if (!quiz.Questions.Any(q => q.Id == existing.Id))
                {
                    _db.QuizQuestions.Remove(existing);
                }
            }
        }


    }
}
