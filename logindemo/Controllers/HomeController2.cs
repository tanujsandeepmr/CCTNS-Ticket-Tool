using logindemo.Data;
using logindemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace logindemo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        
        public AdminController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: /Admin/CCTNSTeam?adminName=VIGNESH
        [HttpGet]
        public IActionResult CCTNSTeam(string adminName = "VIGNESH")
        {
            if (string.IsNullOrWhiteSpace(adminName))
            {
                return BadRequest("Admin name is required.");
            }

            var adminNameLower = adminName.Trim().ToLower();

            IQueryable<Ticket> query;

            if (adminNameLower == "vignesh" || adminNameLower == "pushpa raj"|| adminNameLower == "ram kumar" || adminNameLower == "chandru")
            {
                query = _context.Tickets
                    .Include(t => t.PoliceStationEntity)
                    .Where(t => !string.IsNullOrEmpty(t.CompletedBy) &&
                                t.CompletedBy.ToLower() == adminNameLower);
            }
            else
            {
                // Fallback: Tickets completed by others (i.e., not Vignesh or Pushpa Raj)
                query = _context.Tickets
                    .Include(t => t.PoliceStationEntity)
                    .Where(t => !string.IsNullOrEmpty(t.CompletedBy) &&
                                t.CompletedBy.ToLower() != "vignesh" &&
                                t.CompletedBy.ToLower() != "pushpa raj" &&
                                t.CompletedBy.ToLower() != "ramkumar" &&
                                t.CompletedBy.ToLower() != "chandru");
            }

            var tickets = query.ToList();

            ViewBag.AdminName = adminName.ToUpper();
            ViewBag.TotalCompleted = tickets.Count;

            return View("~/Views/Admin/BackButton.cshtml", tickets);
        }

        // Shortcut: /Admin/VigneshDashboard
        [HttpGet]
        public IActionResult VigneshDashboard()
        {
            return RedirectToAction("CCTNSTeam", new { adminName = "VIGNESH" });
        }

        // Shortcut: /Admin/PushpaRajDashboard
        [HttpGet]
        public IActionResult PushpaRajDashboard()
        {
            return RedirectToAction("CCTNSTeam", new { adminName = "PUSHPA RAJ" });
        }

        // Shortcut: /Admin/RamkumarDashboard
        [HttpGet]
        public IActionResult RamkumarDashboard()
        {
            return RedirectToAction("CCTNSTeam", new { adminName = "RAM KUMAR" });
        }

        // Shortcut: /Admin/RamkumarDashboard
        [HttpGet]
        public IActionResult ChandruDashboard()
        {
            return RedirectToAction("CCTNSTeam", new { adminName = "CHANDRU" });
        }

        // Shortcut: /Admin/CCTNSDashboard
        [HttpGet]
        public IActionResult CCTNSDashboard()
        {
            return RedirectToAction("CCTNSTeam", new { adminName = "CCTNS" });
        }

        // Added: Main Admin Dashboard
        [HttpGet]
        public IActionResult AdminDashboard()
        {
            // You can enhance this with actual summary data if needed
            return View("~/Views/Admin/AdminDashboard.cshtml");
        }
    }
}