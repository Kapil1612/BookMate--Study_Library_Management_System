using BookMate.DataContext;
using BookMate.Models;
using BookMate.Models.ViewModel;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Table = iText.Layout.Element.Table;

namespace BookMate.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }


       

        
        [Authorize(Roles = "Admin")]
        public IActionResult Report(SearchViewModel model)
        {
            // Start building the query
            var query = _context.Students.AsQueryable();

            // Apply filters only if values exist
            if (!string.IsNullOrWhiteSpace(model.StudentSearch))
            {
                query = query.Where(v =>
                    (v.First_Name + " " + v.Last_Name).Contains(model.StudentSearch));
            }

            if (!string.IsNullOrWhiteSpace(model.PaymentSearch))
            {
                query = query.Where(v => v.Payment_Status == model.PaymentSearch);
            }

            if (model.StartDate.HasValue && model.EndDate.HasValue)
            {
                query = query.Where(v =>
                    v.Admission_Date >= model.StartDate &&
                    v.Admission_Date <= model.EndDate);
            }
            else if (model.StartDate.HasValue)
            {
                query = query.Where(v => v.Admission_Date >= model.StartDate);
            }
            else if (model.EndDate.HasValue)
            {
                query = query.Where(v => v.Admission_Date <= model.EndDate);
            }

            var report = new SearchViewModel
            {
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                StudentSearch = model.StudentSearch,
                PaymentSearch = model.PaymentSearch,
                Custom_Count = query.Count(),
                ReportList = query
                    .Select(v => new ReportViewModel
                    {
                        student_ID = v.ID,
                        Student_Name = (v.First_Name ?? "") + " " + (v.Last_Name ?? ""),
                        Contact_Number = v.Contact_Number,
                        No_of_Months = v.No_of_Months,
                        Admission_Date = v.Admission_Date,
                        Fees_Amt = v.Fees_Amt,
                        Total_Fees_Amt = v.Total_Fees_Amt,
                        Payment_Status = v.Payment_Status
                    })
                    .OrderByDescending(v => v.student_ID)
                    .ToList()
            };

            return View(report);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }



        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            ViewBag.CurrentlyCheckedIn = _context.AttendanceLogs.Count(a => a.CheckOutTime == null);
            ViewBag.SeatsOccupied = _context.Seats.Count(s => s.SeatStatus == "Occupied");
            ViewBag.SeatsAvailable = _context.Seats.Count(s => s.SeatStatus == "Available");
            ViewBag.TotalCheckIns = _context.AttendanceLogs.Count(a => a.CheckInTime.HasValue && a.CheckInTime.Value.Date == DateTime.Today);

            ViewBag.RecentLogs = _context.AttendanceLogs
            .Include(v => v.Student)
            .OrderByDescending(v => v.AttendID)
            .Take(5)
            .Select(v => new AttendanceViewModel
            {
                Log_ID = v.AttendID,
                student_ID = v.StudentID,
                Student_Name = v.Student != null ? v.Student.First_Name + " " + v.Student.Last_Name : "",
                SeatID = v.SeatID,
                CheckInTime = v.CheckInTime,
                CheckOutTime = v.CheckOutTime,
                Status = v.Status
            }).ToList();
          
            return View();
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Generate_PDF(SearchViewModel model)
        {
            if ((!model.StartDate.HasValue ) && (!model.EndDate.HasValue)
       && string.IsNullOrEmpty(model.StudentSearch)
       && string.IsNullOrEmpty(model.PaymentSearch))

            {
                TempData["Error"] = "Please select a date range before downloading the custom report.";
                return RedirectToAction("Report");
            }

            else
            {
                var query = _context.Students.AsQueryable();

                if (!string.IsNullOrWhiteSpace(model.StudentSearch))
                {
                    query = query.Where(v =>
                        (v.First_Name + " " + v.Last_Name).Contains(model.StudentSearch));
                }

                if (!string.IsNullOrWhiteSpace(model.PaymentSearch))
                {
                    query = query.Where(v => v.Payment_Status == model.PaymentSearch);
                }

                if (model.StartDate.HasValue && model.EndDate.HasValue)
                {
                    query = query.Where(v =>
                        v.Admission_Date >= model.StartDate &&
                        v.Admission_Date <= model.EndDate);
                }
                else if (model.StartDate.HasValue)
                {
                    query = query.Where(v => v.Admission_Date >= model.StartDate);
                }
                else if (model.EndDate.HasValue)
                {
                    query = query.Where(v => v.Admission_Date <= model.EndDate);
                }

                var result = new SearchViewModel
                {
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    StudentSearch = model.StudentSearch,
                    PaymentSearch = model.PaymentSearch,
                    Custom_Count = query.Count(),
                    ReportList = query
                        .Select(v => new ReportViewModel
                        {
                            student_ID = v.ID,
                            Student_Name = (v.First_Name ?? "") + " " + (v.Last_Name ?? ""),
                            Contact_Number = v.Contact_Number,
                            No_of_Months = v.No_of_Months,
                            Admission_Date = v.Admission_Date,
                            Fees_Amt = v.Fees_Amt,
                            Total_Fees_Amt = v.Total_Fees_Amt,
                            Payment_Status = v.Payment_Status
                        })
                        .OrderByDescending(v => v.student_ID)
                        .ToList()
                };

                byte[] fileBytes;
                using var stream = new MemoryStream();


                using (var writer = new PdfWriter(stream))
                using (var pdf = new PdfDocument(writer))
                using (var document = new Document(pdf))
                {

                    // Title
                    document.Add(new Paragraph("Report")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(18)
                        .SetBold());

                    // Subtitle
                    document.Add(new Paragraph($"Generated on: {DateTime.Now:f}")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(10));
                    document.Add(new Paragraph("\n"));

                    // Filters summary
                    document.Add(new Paragraph("Applied Filters:")
                        .SetBold().SetUnderline());
                    document.Add(new Paragraph(
                        $"Student: {result.StudentSearch ?? "All"} | " +
                        $"Payment: {result.PaymentSearch ?? "All"} | " +
                        $"Date Range: {(result.StartDate?.ToString("dd-MMM-yyyy") ?? "-")} - {(result.EndDate?.ToString("dd-MMM-yyyy") ?? "-")}"
                    ).SetFontSize(10));
                    document.Add(new Paragraph("\n"));


                    if (result.ReportList != null && result.ReportList.Any())
                    {
                       
                        document.Add(Create_Table(result.ReportList));
                    }
                    else
                    {
                        document.Add(new Paragraph("No records found.")
                            .SetTextAlignment(TextAlignment.LEFT)
                            .SetFontSize(12));
                    }


                    document.Close();

                    fileBytes = stream.ToArray();
                }
                return File(fileBytes, "application/pdf", "Student_Report.pdf");

            }
        }



        private Table Create_Table(List<ReportViewModel> rows)
        {
            Table table = new Table(8, false);

            var bold = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

            table.AddHeaderCell(new Cell().Add(new Paragraph("Id").SetFont(bold).SetFontSize(12)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Student Name").SetFont(bold).SetFontSize(12)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Contact Number").SetFont(bold).SetFontSize(12)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("No of Months").SetFont(bold).SetFontSize(12)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Admission Date").SetFont(bold).SetFontSize(12)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Fees").SetFont(bold).SetFontSize(12)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Total Fees").SetFont(bold).SetFontSize(12)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Payment Status").SetFont(bold).SetFontSize(12)));

            foreach (var row in rows)
            {
                table.AddCell(new Cell().Add(new Paragraph(row.student_ID.ToString()).SetFontSize(10)));
                table.AddCell(new Cell().Add(new Paragraph(row.Student_Name ?? "").SetFontSize(10)));
                table.AddCell(new Cell().Add(new Paragraph(row.Contact_Number ?? "").SetFontSize(10)));
                table.AddCell(new Cell().Add(new Paragraph(row.No_of_Months.ToString()).SetFontSize(10)));
                table.AddCell(new Cell().Add(new Paragraph(row.Admission_Date.ToString()).SetFontSize(10)));
                table.AddCell(new Cell().Add(new Paragraph(row.Fees_Amt.ToString()).SetFontSize(10)));
                table.AddCell(new Cell().Add(new Paragraph(row.Total_Fees_Amt.ToString()).SetFontSize(10)));
                table.AddCell(new Cell().Add(new Paragraph(row.Payment_Status ?? "").SetFontSize(10)));
            }

            return table;

        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
