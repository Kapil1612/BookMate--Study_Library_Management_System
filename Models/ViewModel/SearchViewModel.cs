namespace BookMate.Models.ViewModel
{
    public class SearchViewModel
    {


        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? StudentSearch  { get; set; }
        public string? PaymentSearch { get; set; }
        public int Custom_Count { get; set; }

        public List<ReportViewModel> ReportList { get; set; } = new();
    }
}
