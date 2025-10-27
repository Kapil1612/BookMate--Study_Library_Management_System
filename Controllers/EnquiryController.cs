using BookMate.DataContext;
using BookMate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookMate.Controllers
{

    public class EnquiryController : Controller
    {
        private readonly ApplicationDbContext context;

        public EnquiryController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Index(Enquiry model)
        {
            if (ModelState.IsValid)
            {
                context.Enquiries.Add(model);
                context.SaveChanges();
                return RedirectToAction("Enquiry_List");
            }
            return View(model);
        }



        [Authorize(Roles = "Admin")]
        public IActionResult Enquiry_List()
        {
            var enquiry = context.Enquiries.OrderByDescending(v => v.ID).ToList();
            return View(enquiry);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var std = context.Enquiries.Find(id);
            
            if (std != null)
            {
                context.Enquiries.Remove(std);
            }

            context.SaveChanges();
            return RedirectToAction("Enquiry_List");
        }










    }



}
