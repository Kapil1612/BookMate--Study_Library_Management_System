namespace BookMate.Models.ViewModel
{
    public class ReportViewModel
    {

        public int student_ID { get; set; }


        public String? Student_Name { get; set; }



        public string? Contact_Number { get; set; }


        public int No_of_Months { get; set; }


        public DateOnly Admission_Date { get; set; } 


        public decimal Fees_Amt { get; set; }


        public decimal Total_Fees_Amt { get; set; }

        
        public string? Payment_Status { get; set; }


    }
}
