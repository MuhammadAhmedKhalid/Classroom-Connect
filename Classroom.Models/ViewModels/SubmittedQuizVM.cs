namespace Classroom.Models.ViewModels
{
    public class SubmittedQuizVM
    {
        public Quiz quiz { get; set; }
        public List<QuizAnswer>? Answers { get; set; }
    }
}
