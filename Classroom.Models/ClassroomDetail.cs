using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Classroom.Models
{
    public class ClassroomDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Classroom Name cannot exceed 255 characters.")]
        [DisplayName("Classroom Name")]
        public string ClassName { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Creation Date")]
        public DateTime CreationDate { get; set; }
    }
}
