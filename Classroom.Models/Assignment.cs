using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Classroom.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public required string Title { get; set; }
        public string? Instructions { get; set; }

        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }

        public DateTime? PostedAt { get; set; } = DateTime.Now;

        [ForeignKey("Class")]
        public int ClassId { get; set; }
        public Class? Class { get; set; }
    }
}
