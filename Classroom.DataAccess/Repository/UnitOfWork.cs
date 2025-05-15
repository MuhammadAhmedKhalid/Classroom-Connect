using Classroom.DataAccess.Data;
using Classroom.DataAccess.Repository.IRepository;

namespace Classroom.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public IClassRepository Classes { get; private set; }
        public IClassMemberRepository ClassMembers { get; private set; }
        public IAssignmentRepository Assignments { get; private set; }
        public IAssignmentSubmissionRepository AssignmentSubmissions { get; private set; }
        public IQuizRepository Quizzes { get; private set; }
        public IQuizQuestionRepository QuizQuestions { get; private set; }
        public IQuizSubmissionRepository QuizSubmissions { get; private set; }
        public IQuizAnswerRepository QuizAnswers { get; private set; }
        public IAnnouncementRepository Announcements { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Classes = new ClassRepository(_db);
            ClassMembers = new ClassMemberRepository(_db);
            Assignments = new AssignmentRepository(_db);
            AssignmentSubmissions = new AssignmentSubmissionRepository(_db);
            Quizzes = new QuizRepository(_db);
            QuizQuestions = new QuizQuestionRepository(_db);
            QuizSubmissions = new QuizSubmissionRepository(_db);
            QuizAnswers = new QuizAnswerRepository(_db);
            Announcements = new AnnouncementRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
