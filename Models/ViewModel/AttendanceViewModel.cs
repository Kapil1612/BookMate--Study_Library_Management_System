using System.ComponentModel.DataAnnotations;

namespace BookMate.Models.ViewModel
{
    public class AttendanceViewModel
    {
       
        public int Log_ID { get; set; }

        public int student_ID { get; set; }

       

        
        public String? Student_Name { get; set; }


        
        public int SeatID { get; set; }
       


      
        public DateTime? CheckInTime { get; set; } = DateTime.Now;


       
        public DateTime? CheckOutTime { get; set; }


       
        public string Status { get; set; } = "Checked In";
    }
}
