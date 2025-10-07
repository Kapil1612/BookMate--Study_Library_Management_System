using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace BookMate.Models
{
    public class Enquiry
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 50 characters")]
        public String? FirstName { get; set; }
      //  [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 50 characters")]
        public String? LastName { get; set; }

        
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Contact Number is required")]
        [Phone(ErrorMessage = "Enter a valid phone number")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Contact Number must be between 10 and 20 digits")]
        [RegularExpression(@"^[0-9]{10,20}$", ErrorMessage = "Contact Number must contain only digits")]
        [Display(Name = "Contact Number (Mobile)")]
        public string? ContactNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
          ErrorMessage = "Enter a valid email address")]
        public String? Email { get; set; }

    }
}
