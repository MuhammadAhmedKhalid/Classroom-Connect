using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Classroom.Models
{
    public class QuizAnswer
    {
        [Key]
        public int Id { get; set; }

        public int QuestionId { get; set; }
        public string? AnswerText { get; set; }

        [ForeignKey("QuizSubmission")]
        public int QuizSubmissionId { get; set; }
        public QuizSubmission? QuizSubmission { get; set; }
    }
}
