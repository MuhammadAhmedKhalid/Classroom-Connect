using System.ComponentModel.DataAnnotations;

namespace Classroom.Models
{
    public class ClassroomData
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public string ClassCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TeacherId { get; set; } 
    }
}
