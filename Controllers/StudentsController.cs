using BookMate.DataContext;
using BookMate.Models;
using BookMate.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookMate.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }



        // GET: Students
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Students.OrderByDescending(v => v.ID).ToListAsync());
        }

        // GET: Students/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ID,First_Name,Last_Name,Date_of_Birth,Gender,No_of_Months,Admission_Date,Fees_Amt,Total_Fees_Amt,Payment_Status,Student_Status,Contact_Number,Email,Address")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,First_Name,Last_Name,Date_of_Birth,Gender,No_of_Months,Admission_Date,Fees_Amt,Total_Fees_Amt,Payment_Status,Student_Status,Contact_Number,Email,Address")] Student student)
        {
            if (id != student.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        [Authorize(Roles = "Admin")]
        public IActionResult Attendance()
        {
            ViewBag.Seats = _context.Seats
                .Where(s => (s.SeatStatus == "Available") )
                .ToList();
            return View();
        }



        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Attendance(int StudentID, int SeatID, string Status)
        {
           
            if (Status == "CheckIn")
            {

            
                var studentExists = _context.Students.Any(s => s.ID == StudentID);
                if (!studentExists)
                {
                    TempData["Message"] = "❌ Invalid Student ID — student not found in the system.";
                    return RedirectToAction("Attendance"); 
                }

                if (!_context.Seats.Any(s => s.SeatID == SeatID))
                {
                    TempData["Message"] = "❌ Invalid Seat ID — seat does not exist.";
                    return RedirectToAction("Attendance");
                }



                var existingLog = _context.AttendanceLogs
                .Include(a => a.Student)
                .FirstOrDefault(a => a.StudentID == StudentID && a.CheckOutTime == null);

                if (existingLog != null)
                {
                    
                    TempData["Message"] = "⚠️ You have already checked in and not checked out yet.";
                }
                else
                {
                    var log = new AttendanceLog
                    {
                        StudentID = StudentID,
                        SeatID = SeatID,
                        CheckInTime = DateTime.Now,
                        Status = "Checked In"
                    };

                    _context.AttendanceLogs.Add(log);

                    // Mark seat occupied
                    var seat = _context.Seats.Find(SeatID);
                    if (seat != null)
                    {
                        seat.SeatStatus = "Occupied";
                        _context.SaveChanges();
                        TempData["Message"] = "✅ Checked In successfully!";
                    }
                    _context.SaveChanges();

                }
            }
            else if(Status == "CheckOut")
            {
                var log = _context.AttendanceLogs
                .FirstOrDefault(a => a.StudentID == StudentID && a.CheckOutTime == null);

                if (log != null)
                {
                    log.CheckOutTime = DateTime.Now;
                    log.Status = "Checked Out";

                    var seat = _context.Seats.Find(log.SeatID);

                    if (seat != null)
                    {
                        seat.SeatStatus = "Available";
                        _context.SaveChanges();
                        TempData["Message"] = "✅ Checked Out successfully!";
                    }

                }
                else
                {
                    TempData["Message"] = "⚠️ You have already checked Out and not checked In yet.";
                }
            }
            return RedirectToAction("Attendance");
        }


        [Authorize(Roles = "Admin")]
        public IActionResult List_Attendance()
        {
            var model = _context.AttendanceLogs
                .Include(v => v.Student)
                .Include(v => v.Seat)
                .Select(v => new AttendanceViewModel
                {
                    Log_ID = v.AttendID,
                    student_ID = v.StudentID,
                    Student_Name = (v.Student != null ? (v.Student.First_Name ?? "") + " " + (v.Student.Last_Name ?? "") : string.Empty),
                   SeatID = v.SeatID,
                   CheckInTime = v.CheckInTime,
                   CheckOutTime = v.CheckOutTime,
                   Status = v.Status    
                }).OrderByDescending(v => v.Log_ID)
                  .ToList();

            return View(model);
        }





        // Show list of seats
        [Authorize(Roles = "Admin")]
        public IActionResult List_Seat()
        {
            var seats = _context.Seats.OrderByDescending(v => v.SeatID).ToList();
            return View(seats);
        }

      

        // POST: Add new seat
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create_Seat()
        {
            var newSeat = new Seat();
            _context.Seats.Add(newSeat);
            _context.SaveChanges();
            return RedirectToAction("List_Seat");
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete_Seat()
        {
            var lastSeat = _context.Seats
        .OrderByDescending(s => s.SeatID)
        .FirstOrDefault();

            if (lastSeat == null)
            {
                return NotFound();
            }

            _context.Seats.Remove(lastSeat);
            _context.SaveChanges();

            return RedirectToAction("List_Seat");
        }



        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }
    }
}
