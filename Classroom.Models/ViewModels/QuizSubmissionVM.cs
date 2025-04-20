namespace Classroom.Models.ViewModels
{
    public class QuizSubmissionVM
    {
        public int QuizId { get; set; }
        public List<QuizAnswer>? Answers { get; set; }
    }
}
