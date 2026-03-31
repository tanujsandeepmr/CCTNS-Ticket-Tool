using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using logindemo.Models;
using logindemo.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace logindemo.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Feedback/Index
        [Authorize(Roles = "User")]
        public IActionResult Index()
        {
            // FIX: We no longer pass the list to the view because 
            // the new UI is just a single form. 
            // We pass a new empty Feedback object instead.
            return View(new Feedback());
        }

        // POST: Feedback/Submit
        [HttpPost]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int CooperationRating, int MaintenanceRating, string Comments)
        {
            if (CooperationRating == 0 || MaintenanceRating == 0)
            {
                TempData["Error"] = "Please provide ratings for both sections.";
                return RedirectToAction(nameof(Index));
            }

            var feedback = new Feedback
            {
                StationName = User.Identity?.Name ?? "Station User",
                CooperationRating = CooperationRating,
                MaintenanceRating = MaintenanceRating,
                Comments = Comments,
                SubmittedDate = DateTime.Now
            };

            try
            {
                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thank you! Your O&M feedback has been officially recorded.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Database error: Could not save feedback.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Feedback/AdminIndex
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex()
        {
            var feedbackList = await _context.Feedbacks
                .OrderByDescending(f => f.SubmittedDate)
                .ToListAsync();

            return View(feedbackList);
        }
    }
}