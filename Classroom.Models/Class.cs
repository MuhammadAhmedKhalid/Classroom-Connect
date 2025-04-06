using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Classroom.Models
{
    public class Class
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public required string Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; } // Now properly nullable

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("CreatedBy")]
        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
    }
}
