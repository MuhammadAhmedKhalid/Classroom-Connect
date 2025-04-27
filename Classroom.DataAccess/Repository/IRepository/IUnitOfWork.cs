namespace Classroom.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IClassRepository Classes { get; }
        IClassMemberRepository ClassMembers { get; }
        IAssignmentRepository Assignments { get; }
        IAssignmentSubmissionRepository AssignmentSubmissions { get; }
        IQuizRepository Quizzes { get; }
        IQuizQuestionRepository QuizQuestions { get; }
        IQuizSubmissionRepository QuizSubmissions { get; }
        IQuizAnswerRepository QuizAnswers { get; }

        void Save();
    }
}
