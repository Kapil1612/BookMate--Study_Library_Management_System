using System.ComponentModel.DataAnnotations;

namespace BookMate.Models
{
    public class AttendanceLog
    {
        [Key]
        public int AttendID { get; set; }


        [Required]
        public int StudentID { get; set; }
        public Student? Student { get; set; }


        [Required]
        public int SeatID { get; set; }
        public Seat? Seat { get; set; }


        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? CheckInTime { get; set; } = DateTime.Now;


        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? CheckOutTime { get; set; }


        [Required]
        public string Status { get; set; } = "Checked In";
    }
}
