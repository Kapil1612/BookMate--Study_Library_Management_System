using System.ComponentModel.DataAnnotations;

namespace BookMate.Models
{
    public class Seat
    {
        [Key]
        public int SeatID { get; set; }

        [Required]
        public string SeatStatus { get; set; } = "Available";


        public ICollection<AttendanceLog>? AttendanceLogs { get; set; }
    }
}
