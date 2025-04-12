using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Classroom.Models
{
    public class AssignmentSubmission
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Assignment")]
        public int AssignmentId { get; set; }
        public Assignment? Assignment { get; set; } 

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; } 

        public DateTime SubmittedAt { get; set; }

        public string? FilePath { get; set; } 
        public string? FileName { get; set; } 
    }
}
