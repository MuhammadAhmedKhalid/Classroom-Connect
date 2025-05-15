using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Classroom.Models
{
    public class Announcement
    {
        public int Id { get; set; }

        [Required]
        public required string ContentHtml { get; set; }

        public DateTime? PostedAt { get; set; } = DateTime.Now;

        [ForeignKey("Class")]
        public int ClassId { get; set; }
        public Class? Class { get; set; }
    }
}
