using System.ComponentModel.DataAnnotations;

namespace Classroom.Models.ViewModels
{
    public class CreateClassVM
    {
        [Required(ErrorMessage = "Class name is required")]
        [Display(Name = "Class Name")]
        [StringLength(100, ErrorMessage = "Class name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
    }

}
