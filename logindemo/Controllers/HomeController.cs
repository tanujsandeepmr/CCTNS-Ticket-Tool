/*using logindemo.Data;
using logindemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace logindemo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        [Route("Home/Index")]
        [Route("AdminDashboard")]
        public async Task<IActionResult> Index()
        {
            var tickets = await _context.Tickets
                .Include(t => t.PoliceStationEntity)
                .ToListAsync();

            var warningTickets = tickets
                .Where(t => t.Priority == "Urgent" &&
                            t.CreatedAt >= DateTime.Now.AddDays(-2) &&
                            t.Status != "Completed")
                .ToList();
            ViewBag.WarningTickets = warningTickets;

            var mediumPriorityTickets = tickets
                .Where(t => t.Priority == "Medium" &&
                            (DateTime.Now - t.CreatedAt).TotalHours >= 48 &&
                            t.Status != "Completed")
                .ToList();
            ViewBag.MediumPriorityTickets = mediumPriorityTickets;

            var highPriorityTickets = tickets
                .Where(t => t.Priority == "High" &&
                            (DateTime.Now - t.CreatedAt).TotalHours >= 48 &&
                            t.Status != "Completed")
                .ToList();
            ViewBag.HighPriorityTickets = highPriorityTickets;

            var lowPriorityTickets = tickets
                .Where(t => t.Priority == "Low" &&
                            (DateTime.Now - t.CreatedAt).TotalHours >= 48 &&
                            t.Status != "Completed")
                .ToList();
            ViewBag.LowPriorityTickets = lowPriorityTickets;

            return View(tickets);
        }

        [Route("UserDashboard")]
        public async Task<IActionResult> UserDashboard()
        {
            var loggedUser = await _userManager.GetUserAsync(User);

            if (loggedUser == null)
                return RedirectToAction("Login", "Account");

            var stationCode = loggedUser.PoliceStationCode;

            // Fetch tickets for this user's police station
            var tickets = await _context.Tickets
                .Where(t => t.PoliceStationCode == stationCode)
               .OrderByDescending(t => t.TicketId)
                .ToListAsync();

            ViewBag.TotalTickets = tickets.Count;
            ViewBag.PendingTickets = tickets.Count(t => t.Status == "Pending" || t.Status == "Open");
            ViewBag.CompletedTickets = tickets.Count(t => t.Status == "Completed");

            return View(tickets);
        }


        [HttpPost]
        public async Task<IActionResult> MarkAsCompleted(string ticketId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Forbid();

            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketId == ticketId);
            if (ticket == null) return NotFound();

            ticket.Status = "Completed";
            ticket.CompletedAt = DateTime.Today;
            ticket.CompletedTime = DateTime.Now; // ✅ Add this line
            ticket.CompletedBy = user?.UserName ?? "UNKNOWN";

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // or return Json if it's an AJAX call
        }


        [HttpGet]
        public async Task<IActionResult> GetChartData()
        {
            var now = DateTime.Now;
            var currentMonth = now.Month;
            var currentYear = now.Year;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);

            var monthly = await _context.Tickets
                .Where(t => t.CreatedAt.Month == currentMonth && t.CreatedAt.Year == currentYear)
                .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
                .Select(g => new
                {
                    Label = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                    Count = g.Count()
                })
                .OrderBy(g => g.Label)
                .ToListAsync();

            var weekly = await _context.Tickets
                .Where(t => t.CreatedAt >= startOfWeek && t.CreatedAt < endOfWeek)
                .GroupBy(t => t.Priority)
                .Select(g => new
                {
                    Priority = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var yearly = await _context.Tickets
                .Where(t => t.CreatedAt.Year == currentYear)
                .GroupBy(t => t.CreatedAt.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Count = g.Count()
                })
                .OrderBy(g => g.Year)
                .ToListAsync();

            return Json(new
            {
                Monthly = monthly,
                Weekly = weekly,
                Yearly = yearly
            });
        }

        public IActionResult GetMonthlyTicketCounts()
        {
            var allMonths = Enumerable.Range(1, 12).ToDictionary(m => m, m => 0);

            var ticketCounts = _context.Tickets
                .GroupBy(t => t.CreatedAt.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToList();

            foreach (var item in ticketCounts)
                allMonths[item.Month] = item.Count;

            var result = allMonths
                .Select(kvp => new { month = kvp.Key, count = kvp.Value })
                .ToList();

            return Json(result);
        }

        public IActionResult GetWeeklyTicketCounts(int month)
        {
            var weeklyCounts = _context.Tickets
                .Where(t => t.CreatedAt.Month == month)
                .ToList()
                .GroupBy(t => (t.CreatedAt.Day - 1) / 7 + 1)
                .Select(g => new { week = g.Key, count = g.Count() })
                .ToList();

            return Json(weeklyCounts);
        }

        [HttpGet]
        [Route("Admin/CCTNSTeam")]
        public async Task<IActionResult> CCTNSTeam()
        {
            var currentAdmin = User?.Identity?.Name?.ToUpperInvariant();
            if (string.IsNullOrEmpty(currentAdmin))
                return Unauthorized();

            var tickets = await _context.Tickets
                .Where(t => t.Status == "Completed"
                         && t.CompletedBy != null
                         && t.CompletedBy.ToUpper() == currentAdmin)
                .OrderByDescending(t => t.CompletedAt)
                .ToListAsync();

            ViewBag.AdminName = currentAdmin;
            return View(tickets);
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("Admin/ExportTicketsToExcel")]
        public IActionResult ExportTicketsToExcel(string policeStation, string issue, DateTime from, DateTime to)
        {
            var ticketsQuery = _context.Tickets
                .Include(t => t.PoliceStationEntity)
                .Where(t => t.CreatedAt >= from && t.CreatedAt <= to);

            if (!string.IsNullOrWhiteSpace(policeStation))
            {
                ticketsQuery = ticketsQuery.Where(t =>
                    t.PoliceStationEntity != null &&
                    t.PoliceStationEntity.StationName.Contains(policeStation));
            }

            if (!string.IsNullOrWhiteSpace(issue))
            {
                ticketsQuery = ticketsQuery.Where(t =>
                    !string.IsNullOrEmpty(t.Issue) &&
                    t.Issue.Contains(issue));
            }

            var tickets = ticketsQuery.ToList();

            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Tickets");

            // ✅ Headers
            worksheet.Cells[1, 1].Value = "Ticket ID";
            worksheet.Cells[1, 2].Value = "Reporter";
            worksheet.Cells[1, 3].Value = "Police Station";
            worksheet.Cells[1, 4].Value = "Region";
            worksheet.Cells[1, 5].Value = "Issue";
            worksheet.Cells[1, 6].Value = "Status";
            worksheet.Cells[1, 7].Value = "Priority";
            worksheet.Cells[1, 8].Value = "Created At";
            worksheet.Cells[1, 9].Value = "Completed Time";
            worksheet.Cells[1, 10].Value = "Action Taken";

            // ✅ Rows
            int row = 2;
            foreach (var t in tickets)
            {
                worksheet.Cells[row, 1].Value = t.TicketId ?? "";
                worksheet.Cells[row, 2].Value = t.ReporterName ?? "";
                worksheet.Cells[row, 3].Value = t.PoliceStationEntity?.StationName ?? "";
                worksheet.Cells[row, 4].Value = t.PoliceStationEntity?.Region ?? "";
                worksheet.Cells[row, 5].Value = t.Issue ?? "";
                worksheet.Cells[row, 6].Value = t.Status ?? "";
                worksheet.Cells[row, 7].Value = t.Priority ?? "";
                worksheet.Cells[row, 8].Value = t.CreatedAt.ToString("dd-MM-yyyy HH:mm");

                worksheet.Cells[row, 9].Value = t.CompletedTime.HasValue
                    ? t.CompletedTime.Value.ToString("dd-MM-yyyy hh:mm tt")
                    : "";

                worksheet.Cells[row, 10].Value = ""; // Active (blank)
                row++;
            }

            // ✅ Auto-fit all columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            string fileName = $"Tickets_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }




    }
}



*/




using logindemo.Data;
using logindemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace logindemo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        [Route("Home/Index")]
        [Route("AdminDashboard")]
        public async Task<IActionResult> Index()
        {
            var tickets = await _context.Tickets
                .Include(t => t.PoliceStationEntity)
                .ToListAsync();

            // 1. PRIMARY FIX: Combine Open and Re-Opened for the main "Open Cases" card
            // Use this variable in your HTML for the Red Summary Card
            ViewBag.OpenCasesCount = tickets.Count(t => t.Status == "Open" || t.Status == "Re-Opened");

            // 2. Tracking tickets waiting for Police Station response
            ViewBag.AdminPendingFromPS = tickets.Count(t => t.Status == "Pending from Police Station");

            // 3. Keep specific counts for other cards if needed
            ViewBag.ReOpenedCount = tickets.Count(t => t.Status == "Re-Opened");
            ViewBag.CompletedCount = tickets.Count(t => t.Status == "Completed");
            ViewBag.TotalTickets = tickets.Count;

            var warningTickets = tickets
                .Where(t => t.Priority == "Urgent" &&
                            t.CreatedAt >= DateTime.Now.AddDays(-2) &&
                            t.Status != "Completed")
                .ToList();
            ViewBag.WarningTickets = warningTickets;

            var mediumPriorityTickets = tickets
                .Where(t => t.Priority == "Medium" &&
                            (DateTime.Now - t.CreatedAt).TotalHours >= 48 &&
                            t.Status != "Completed")
                .ToList();
            ViewBag.MediumPriorityTickets = mediumPriorityTickets;

            var highPriorityTickets = tickets
                .Where(t => t.Priority == "High" &&
                            (DateTime.Now - t.CreatedAt).TotalHours >= 48 &&
                            t.Status != "Completed")
                .ToList();
            ViewBag.HighPriorityTickets = highPriorityTickets;

            var lowPriorityTickets = tickets
                .Where(t => t.Priority == "Low" &&
                            (DateTime.Now - t.CreatedAt).TotalHours >= 48 &&
                            t.Status != "Completed")
                .ToList();
            ViewBag.LowPriorityTickets = lowPriorityTickets;

            return View(tickets);
        }

        // NOTE: [Route("UserDashboard")] was removed from here 
        // because it is now handled by UserDashboardController.cs

        [HttpPost]
        public async Task<IActionResult> MarkAsCompleted(string ticketId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Forbid();

            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketId == ticketId);
            if (ticket == null) return NotFound();

            ticket.Status = "Completed";
            ticket.CompletedAt = DateTime.Today;
            ticket.CompletedTime = DateTime.Now;
            ticket.CompletedBy = user?.UserName ?? "UNKNOWN";

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetChartData()
        {
            var now = DateTime.Now;
            var currentMonth = now.Month;
            var currentYear = now.Year;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);

            var monthly = await _context.Tickets
                .Where(t => t.CreatedAt.Month == currentMonth && t.CreatedAt.Year == currentYear)
                .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
                .Select(g => new
                {
                    Label = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                    Count = g.Count()
                })
                .OrderBy(g => g.Label)
                .ToListAsync();

            var weekly = await _context.Tickets
                .Where(t => t.CreatedAt >= startOfWeek && t.CreatedAt < endOfWeek)
                .GroupBy(t => t.Priority)
                .Select(g => new
                {
                    Priority = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var yearly = await _context.Tickets
                .Where(t => t.CreatedAt.Year == currentYear)
                .GroupBy(t => t.CreatedAt.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Count = g.Count()
                })
                .OrderBy(g => g.Year)
                .ToListAsync();

            return Json(new
            {
                Monthly = monthly,
                Weekly = weekly,
                Yearly = yearly
            });
        }

        public IActionResult GetMonthlyTicketCounts()
        {
            var allMonths = Enumerable.Range(1, 12).ToDictionary(m => m, m => 0);

            var ticketCounts = _context.Tickets
                .GroupBy(t => t.CreatedAt.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToList();

            foreach (var item in ticketCounts)
                allMonths[item.Month] = item.Count;

            var result = allMonths
                .Select(kvp => new { month = kvp.Key, count = kvp.Value })
                .ToList();

            return Json(result);
        }

        public IActionResult GetWeeklyTicketCounts(int month)
        {
            var weeklyCounts = _context.Tickets
                .Where(t => t.CreatedAt.Month == month)
                .ToList()
                .GroupBy(t => (t.CreatedAt.Day - 1) / 7 + 1)
                .Select(g => new { week = g.Key, count = g.Count() })
                .ToList();

            return Json(weeklyCounts);
        }

        [HttpGet]
        [Route("Admin/CCTNSTeam")]
        public async Task<IActionResult> CCTNSTeam()
        {
            var currentAdmin = User?.Identity?.Name?.ToUpperInvariant();
            if (string.IsNullOrEmpty(currentAdmin))
                return Unauthorized();

            var tickets = await _context.Tickets
                .Where(t => t.Status == "Completed"
                         && t.CompletedBy != null
                         && t.CompletedBy.ToUpper() == currentAdmin)
                .OrderByDescending(t => t.CompletedAt)
                .ToListAsync();

            ViewBag.AdminName = currentAdmin;
            return View(tickets);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("Admin/ExportTicketsToExcel")]
        public IActionResult ExportTicketsToExcel(string policeStation, string issue, DateTime from, DateTime to)
        {
            // Ensure the 'to' date includes the entire day up to 11:59:59 PM
            DateTime endDate = to.Date.AddDays(1).AddTicks(-1);

            var ticketsQuery = _context.Tickets
                .Include(t => t.PoliceStationEntity)
                .Where(t => t.CreatedAt >= from.Date && t.CreatedAt <= endDate);

            if (!string.IsNullOrWhiteSpace(policeStation))
            {
                ticketsQuery = ticketsQuery.Where(t =>
                    t.PoliceStationEntity != null &&
                    t.PoliceStationEntity.StationName.Contains(policeStation));
            }

            if (!string.IsNullOrWhiteSpace(issue))
            {
                ticketsQuery = ticketsQuery.Where(t =>
                    !string.IsNullOrEmpty(t.Issue) &&
                    t.Issue.Contains(issue));
            }

            // 🔥 FIX: Sort purely by Ticket ID to ensure they are in a perfect sequence
            var tickets = ticketsQuery.OrderBy(t => t.TicketId).ToList();

            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Tickets");

            // Headers (Added S.No for better tracking)
            worksheet.Cells[1, 1].Value = "S.No";
            worksheet.Cells[1, 2].Value = "Ticket ID";
            worksheet.Cells[1, 3].Value = "Reporter";
            worksheet.Cells[1, 4].Value = "Police Station";
            worksheet.Cells[1, 5].Value = "Region";
            worksheet.Cells[1, 6].Value = "Issue";
            worksheet.Cells[1, 7].Value = "Status";
            worksheet.Cells[1, 8].Value = "Priority";
            worksheet.Cells[1, 9].Value = "Created At";
            worksheet.Cells[1, 10].Value = "Completed Time";
            worksheet.Cells[1, 11].Value = "Action Taken";

            // Styling Headers
            using (var range = worksheet.Cells[1, 1, 1, 11])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }

            int row = 2;
            int serialNo = 1;
            foreach (var t in tickets)
            {
                worksheet.Cells[row, 1].Value = serialNo++;
                worksheet.Cells[row, 2].Value = t.TicketId ?? "";
                worksheet.Cells[row, 3].Value = t.ReporterName ?? "";
                worksheet.Cells[row, 4].Value = t.PoliceStationEntity?.StationName ?? "";
                worksheet.Cells[row, 5].Value = t.PoliceStationEntity?.Region ?? "";
                worksheet.Cells[row, 6].Value = t.Issue ?? "";
                worksheet.Cells[row, 7].Value = t.Status ?? "";
                worksheet.Cells[row, 8].Value = t.Priority ?? "";
                worksheet.Cells[row, 9].Value = t.CreatedAt.ToString("dd-MM-yyyy HH:mm");

                worksheet.Cells[row, 10].Value = t.CompletedTime.HasValue
                    ? t.CompletedTime.Value.ToString("dd-MM-yyyy hh:mm tt")
                    : "";

                worksheet.Cells[row, 11].Value = ""; // Action Taken Column
                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            string fileName = $"Tickets_Report_{from:yyyyMMdd}_to_{to:yyyyMMdd}.xlsx";

            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }
    }
}