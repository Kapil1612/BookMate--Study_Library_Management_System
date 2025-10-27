using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookMate.Models
{
    public class Student
    {
        [Key]
        public int ID { get; set; }


        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 50 characters")]
        
        public String? First_Name { get; set; }


       // [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 50 characters")]
       
        public String? Last_Name { get; set; }

        
        public DateOnly Date_of_Birth { get; set; }


        [Required(ErrorMessage = "Gender is required")]
        public string? Gender { get; set; }

       
        [Required(ErrorMessage = "Duration is required")]
        public int No_of_Months { get; set; }

       
        public DateOnly Admission_Date { get; set; } 


        [Precision(18, 2)]
        public decimal Fees_Amt { get; set; }

        [Precision(18, 2)]
        public decimal Total_Fees_Amt { get; set; }

        [Required]
        public string? Payment_Status { get; set; }

        public string? Student_Status { get; set; } = "Active";


        [Required(ErrorMessage = "Contact Number is required")]
        [Phone(ErrorMessage = "Enter a valid phone number")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Contact Number must be between 10 and 20 digits")]
        [RegularExpression(@"^[0-9]{10,20}$", ErrorMessage = "Contact Number must contain only digits")]
        public string? Contact_Number { get; set; }



        [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
          ErrorMessage = "Enter a valid email address")]
        public String? Email { get; set; }

        [Required]
        public string? Address { get; set; }


        public ICollection<AttendanceLog>? AttendanceLogs { get; set; }
    }
}

