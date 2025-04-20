using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Classroom.Models
{
    public class QuizSubmission
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Quiz")]
        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public DateTime SubmittedAt { get; set; }
    }
}
