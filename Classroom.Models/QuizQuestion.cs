using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Classroom.Models
{
    public class QuizQuestion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Question text is required.")]
        public required string Text { get; set; }

        public string? CorrectAnswer { get; set; }

        [ForeignKey("Quiz")]
        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }
    }
}
