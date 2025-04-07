using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Classroom.Models
{
    public class ClassMember
    {
        [Key]
        public int ClassMemberId { get; set; } // Primary Key for this linking table

        [ForeignKey("Class")]
        public int ClassId { get; set; }
        public Class? Class { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public DateTime JoinedAt { get; set; }
    }
}
