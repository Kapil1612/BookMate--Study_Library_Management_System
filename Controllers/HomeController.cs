using BookMate.DataContext;
using BookMate.Models;
using BookMate.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
        public IActionResult Index()
        {
            return View();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
