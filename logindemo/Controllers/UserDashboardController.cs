using logindemo.Data;
using logindemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace logindemo.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class UserDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserDashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Main user ticket list view with optional filters
        public async Task<IActionResult> Index(string status, string priority)
        {
            var tickets = _context.Tickets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                tickets = tickets.Where(t => t.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(priority))
            {
                tickets = tickets.Where(t => t.Priority == priority);
            }

            ViewBag.Status = status;
            ViewBag.Priority = priority;

            return View(await tickets.ToListAsync());
        }



        [Route("UserDashboard")]
        public async Task<IActionResult> Index()
        {
            var loggedUser = await _userManager.GetUserAsync(User);

            if (loggedUser == null)
                return RedirectToAction("Login", "Account");

            var stationCode = loggedUser.PoliceStationCode;

            var tickets = await _context.Tickets
                .Where(t => t.PoliceStationCode == stationCode)
                .OrderByDescending(t => t.TicketId)
                .ToListAsync();

            // Summary Card Counts
            ViewBag.TotalTickets = tickets.Count;
            ViewBag.PendingFromPS = tickets.Count(t => t.Status == "Pending from Police Station");
            //ViewBag.PendingTickets = tickets.Count(t => t.Status == "Pending" || t.Status == "Open");
            //ViewBag.CompletedTickets = tickets.Count(t => t.Status == "Completed");

            // 🔥 UPDATE: Include "Re-Opened" in the Pending count
            ViewBag.PendingTickets = tickets.Count(t =>
                t.Status == "Pending" ||
                t.Status == "Open" ||
                t.Status == "Re-Opened");

            // This line tells the controller to look in the Home folder instead of its own
            return View("~/Views/Home/UserDashboard.cshtml", tickets);
        }

        // Role/name-based view routing for dashboards
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userName = user.UserName?.Trim().ToUpperInvariant();

            return userName switch
            {
                "VIGNESH" => View("VigneshDashboard"),
                "PUSHPA RAJ" => View("PushpaDashboard"),
                "RAMKUMAR" => View("RamkumarDashboard"),
                "CHANDRU" => View("ChandruDashboard"),
                "CCTNSTEAM" => View("CCTNSDashboard"),
                _ => View("DefaultDashboard")
            };
        }
    }
}
