using System.ComponentModel.DataAnnotations;

namespace Classroom.Models.ViewModels
{
    public class JoinClassVM
    {
        [Required(ErrorMessage = "Class Code is required.")]
        [StringLength(8, MinimumLength = 5, ErrorMessage = "Class Code must be between 5 and 8 characters.")]
        [Display(Name = "Class Code")]
        public required string ClassCode { get; set; }
    }
}
