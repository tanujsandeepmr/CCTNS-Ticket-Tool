using logindemo.Data;
using logindemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace logindemo.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;


        public TicketController(ApplicationDbContext context, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }

        /* public async Task<IActionResult> UserDashboard()
         {
             var userId = _userManager.GetUserId(User);



             if (string.IsNullOrEmpty(userId))
                 return RedirectToAction("Login", "Account");

             // Fetch tickets ONLY created by this user
             var tickets = await _context.Tickets
                 .Where(t => t.UserId == userId)
                 .OrderByDescending(t => t.CreatedAt)
                 .ToListAsync();

             *//*  ViewBag.TotalTickets = tickets.Count;

               // FIX: Include all pending types
               ViewBag.PendingTickets = tickets.Count(t =>
                   t.Status == "Pending" || t.Status == "Open" || t.Status == "In Progress"
               );

               ViewBag.CompletedTickets = tickets.Count(t => t.Status == "Completed");*//*

             // Inside UserDashboard Action
             ViewBag.TotalTickets = tickets.Count;
             ViewBag.CompletedTickets = tickets.Count(t => t.Status == "Completed");

             // Check if this matches your DB status string exactly!
             ViewBag.Pendingfrompolicestation = tickets.Count(t => t.Status == "Pending from Police Station");

             // General pending (excluding the one above)
             ViewBag.PendingTickets = tickets.Count(t => t.Status == "Pending" || t.Status == "Open");

             return View(tickets);
         }
 */


        /*  public async Task<IActionResult> UserDashboard()
          {
              var userId = _userManager.GetUserId(User);
              if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

              var tickets = await _context.Tickets
                  .Where(t => t.UserId == userId)
                  .OrderByDescending(t => t.Status == "Pending from Police Station")
                  .ThenByDescending(t => t.CreatedAt)
                  .ToListAsync();

              ViewBag.TotalTickets = tickets.Count;
              ViewBag.PendingFromPS = tickets.Count(t => t.Status == "Pending from Police Station");
              ViewBag.PendingTickets = tickets.Count(t => t.Status == "Pending" || t.Status == "Open");
              ViewBag.CompletedTickets = tickets.Count(t => t.Status == "Completed");

              // FIX: Pass 'tickets' to the view
              return View(tickets);
          }*/

        public async Task<IActionResult> UserDashboard()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

            var query = _context.Tickets.Where(t => t.UserId == userId);

            // Calculate directly from Database to avoid local list mismatches
            ViewBag.TotalTickets = await query.CountAsync();
            ViewBag.CompletedTickets = await query.CountAsync(t => t.Status == "Completed");
            ViewBag.PendingFromPS = await query.CountAsync(t => t.Status == "Pending from Police Station");
            ViewBag.PendingTickets = await query.CountAsync(t => t.Status == "Pending" || t.Status == "Open" || t.Status == "Assigned" || t.Status == "Re-Opened");

            var tickets = await query
                .OrderByDescending(t => t.Status == "Pending from Police Station")
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }
        public async Task<IActionResult> PoliceDashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.PoliceStationCode))
                return Unauthorized();

            var tickets = await _context.Tickets
                .Where(t => t.PoliceStationCode == user.PoliceStationCode)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.StationCode = user.PoliceStationCode;

            ViewBag.TotalTickets = tickets.Count;

            // IMPORTANT FIX
            ViewBag.PendingTickets = tickets.Count(t =>
                t.Status == "Pending" ||
                t.Status == "Open" ||
                t.Status == "In Progress"
            );

            ViewBag.CompletedTickets = tickets.Count(t => t.Status == "Completed");

            return View("PoliceDashboard", tickets);
        }



        public async Task<IActionResult> Create()
        {
            var stations = _context.PoliceStations.OrderBy(p => p.StationName).ToList();
            ViewBag.PoliceStations = stations; // Admin will use this
            ViewBag.Issues = GetIssues();

            if (!User.IsInRole("Admin"))
            {
                // Get logged-in user
                var user = await _userManager.GetUserAsync(User);
                if (user != null && !string.IsNullOrEmpty(user.PoliceStationCode))
                {
                    var userStation = stations.FirstOrDefault(s =>
                            s.StationCode.Trim().ToUpper() ==
                            user.PoliceStationCode.Trim().ToUpper());


                    if (userStation != null)
                    {
                        ViewBag.UserStationCode = userStation.StationCode;
                        ViewBag.PoliceStationName = userStation.StationName;
                        ViewBag.UserRegion = userStation.Region;
                    }
                    else
                    {
                        ViewBag.UserStationCode = "";
                        ViewBag.PoliceStationName = "";
                        ViewBag.UserRegion = "";
                    }
                }
            }

            var ticket = new Ticket
            {
                TicketId = "Will be generated",
                CreatedAt = DateTime.Now,
                Status = "Open"
            };

            return View(ticket);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PoliceStations = _context.PoliceStations.OrderBy(p => p.StationName).ToList();
                ViewBag.Issues = GetIssues();
                return View(ticket);
            }

            ticket.TicketId = await GenerateTicketIdAsync();
           // ticket.TicketId = await GenerateTicketIdAsync(ticket.PoliceStationCode);
            ticket.Priority = GetPriorityByIssue(ticket.Issue);
            ticket.Status = "Open";
            ticket.UserId = _userManager.GetUserId(User);

            if (User.IsInRole("Admin"))
            {
                ticket.CreatedDate = ticket.CreatedAt.Date;

                if (ticket.CompletedAt.HasValue)
                {
                    ticket.CompletedTime = ticket.CompletedAt.Value;
                    ticket.Status = "Completed";
                }
            }
            else
            {
                ticket.CreatedAt = DateTime.Now;
                ticket.CreatedDate = DateTime.Today;
                ticket.CompletedAt = null;
                ticket.CompletedTime = null;
                ticket.CompletedBy = null;
            }


            var station = await _context.PoliceStations.FirstOrDefaultAsync(ps => ps.StationCode == ticket.PoliceStationCode);
            if (station != null)
            {
                ticket.PoliceStationName = station.StationName;
                ticket.PoliceStationId = station.Id;

                var karaikalStations = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "FOOD CELL PS KARAIKAL", "KOTTUCHERRY", "TOWN KARAIKAL", "NEDUNGADU", "NERAVY", "THIRUNALLAR",
                "T.R.PATTINAM", "WOMEN PS KARAIKAL", "PCR CELL PS KARAIKAL", "TRAFFIC NORTH",
                "EXCISE PS KARAIKAL", "COASTAL PS KARAIKAL", "TRAFFIC SOUTH"
            };

                ticket.Region = karaikalStations.Contains(station.StationName?.Trim() ?? "")
                    ? "KARAIKAL"
                    : "PUDUCHERRY";
            }



            if (ticket.ImageFile != null && ticket.ImageFile.Length > 0)
            {
                string uniqueFileName = await UploadImageAsync(ticket.ImageFile);
                ticket.ImagePath = "/uploads/" + uniqueFileName;
            }

            if (ticket.ApprovalFiles != null && ticket.ApprovalFiles.Count > 0)
            {
                List<string> filePaths = new List<string>();
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                foreach (var file in ticket.ApprovalFiles)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    filePaths.Add("~/uploads/" + uniqueFileName);
                }

                // Store as comma-separated paths in DB
                ticket.ApprovalDocuments = string.Join(",", filePaths);
            }


            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ticket submitted successfully!";
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("UserDashboard", "Tickets");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsCompleted([FromForm] int id) // Add [FromForm]
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Forbid();

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            if (ticket.Status != "Completed")
            {
                ticket.Status = "Completed";
                ticket.CompletedAt = DateTime.Now;
                ticket.CompletedTime = DateTime.Now;
                ticket.CompletedBy = user.UserName ?? "";

                _context.Tickets.Update(ticket);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Ticket completed successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetTicketByTicketId(string ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.PoliceStationEntity)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket == null) return NotFound();

            return Json(new
            {
                ticket.TicketId,
                ticket.PoliceStationName,
                ticket.ReporterName,
                ticket.Issue,
                ticket.Priority,
                CreatedAt = ticket.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                ticket.Status
            });
        }

        public async Task<IActionResult> Reopen(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ticket.Status = "Open";
            ticket.CompletedBy = null;
            ticket.CompletedAt = null;

            await _context.SaveChangesAsync();
            return RedirectToAction("PoliceDashboard");
        }

        /*     [Authorize(Roles = "User")]
             public async Task<IActionResult> Index()
             {
                 var userId = _userManager.GetUserId(User);

                 var tickets = await _context.Tickets
                     .Where(t => t.UserId == userId)

                     .Include(t => t.PoliceStationEntity)
                     .OrderByDescending(t => t.CreatedAt)
                     .ToListAsync();

                 return View(tickets);
             }
     */

        public async Task<IActionResult> Index()
        {
            // 1. Get the current logged-in user
            var user = await _userManager.GetUserAsync(User);

            // 2. Get the Station Code assigned to this user
            // Note: Ensure your ApplicationUser model has a property called PoliceStationCode
            var userStationCode = user.PoliceStationCode;

            // 3. Fetch all tickets belonging to that station
            var tickets = await _context.Tickets
                .Where(t => t.PoliceStationCode == userStationCode) // Filter by Station, not just UserID
                .Include(t => t.PoliceStationEntity)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        public async Task<IActionResult> CreatedTickets()
        {
            var tickets = await _context.Tickets.ToListAsync();
            return View(tickets);
        }

        public IActionResult ViewTicket(int id)
        {
            var ticket = _context.Tickets.Find(id);
            return View(ticket);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCompletionPdf(int ticketId, IFormFile pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a valid PDF file.";
                return RedirectToAction("ViewTicket", new { id = ticketId });
            }

            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return NotFound();

            // Ensure upload folder exists
            string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads/PDF");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            // Generate unique filename
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(pdfFile.FileName);
            string filePath = Path.Combine(uploadFolder, uniqueFileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await pdfFile.CopyToAsync(stream);
            }

            // Save relative PDF path in DB
            ticket.PdfPath = "/uploads/PDF/" + uniqueFileName;

            // Mark ticket as completed
            ticket.Status = "Completed";
            /* ticket.CompletedAt = DateTime.Now;
             ticket.CompletedTime = DateTime.Now;*/

            var user = await _userManager.GetUserAsync(User);
            ticket.CompletedBy = user?.UserName ?? "System";

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "PDF uploaded and ticket marked as completed!";
            return RedirectToAction("ViewTicket", new { id = ticketId });
        }

        // 1. GET: OPEN THE EDIT PAGE
        [HttpGet]
        public async Task<IActionResult> EditTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            if (ticket.Status == "Completed")
            {
                TempData["ErrorMessage"] = "Completed tickets cannot be edited.";
                return RedirectToAction("ViewUserTicket", new { id = id });
            }

            ViewBag.Issues = GetIssues();

            // LOGIC FOR FIXED STATION/REGION
            if (!User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                var station = await _context.PoliceStations
                    .FirstOrDefaultAsync(s => s.StationCode == user.PoliceStationCode);

                if (station != null)
                {
                    ViewBag.FixedStationName = station.StationName;
                    ViewBag.FixedRegion = station.Region;
                    ViewBag.FixedStationCode = station.StationCode;
                }
            }
            else
            {
                // Admin can still see all stations
                ViewBag.PoliceStations = _context.PoliceStations.OrderBy(p => p.StationName).ToList();
            }

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTicket(int id, Ticket updatedTicket)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            // 1. Assign the Station Code
            ticket.PoliceStationCode = updatedTicket.PoliceStationCode;

            // 2. Refresh Station Name and Region
            var station = await _context.PoliceStations
                .FirstOrDefaultAsync(s => s.StationCode == ticket.PoliceStationCode);

            if (station != null)
            {
                ticket.PoliceStationName = station.StationName;
                ticket.Region = station.Region;
            }

            // 3. Update text fields
            ticket.ReporterName = updatedTicket.ReporterName;
            ticket.Issue = updatedTicket.Issue;
            ticket.Description = updatedTicket.Description;

            // Optional: Only update CreatedAt if you want it to jump to the top 
            // of the "Latest" list upon every edit.
            ticket.CreatedAt = DateTime.Now;

            // 4. Recalculate Priority
            ticket.Priority = GetPriorityByIssue(updatedTicket.Issue);

            try
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Ticket updated successfully!";
            }
            catch (Exception ex)
            {
                // IMPORTANT FIX: Re-populate ViewBag so the dropdowns don't break on the return
                ViewBag.Issues = GetIssues();
                if (User.IsInRole("Admin"))
                {
                    ViewBag.PoliceStations = _context.PoliceStations.OrderBy(p => p.StationName).ToList();
                }
                else
                {
                    ViewBag.FixedStationName = ticket.PoliceStationName;
                    ViewBag.FixedRegion = ticket.Region;
                    ViewBag.FixedStationCode = ticket.PoliceStationCode;
                }

                TempData["ErrorMessage"] = "Error updating ticket: " + ex.Message;
                return View(updatedTicket);
            }

            return RedirectToAction("ViewUserTicket", new { id = ticket.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var ticketToDelete = await _context.Tickets.FindAsync(id);
            if (ticketToDelete == null) return NotFound();

            if (ticketToDelete.Status == "Completed")
            {
                TempData["ErrorMessage"] = "You cannot delete a completed ticket.";
                return RedirectToAction("ViewUserTicket", new { id = id });
            }

            // 1. Capture the numeric value of the ticket being deleted
            string prefix = "CCTNS";
            if (!long.TryParse(ticketToDelete.TicketId.Replace(prefix, ""), out long deletedNum))
            {
                return BadRequest("Invalid Ticket ID format.");
            }

            // 2. Delete the record
            _context.Tickets.Remove(ticketToDelete);
            await _context.SaveChangesAsync();

            // 3. Update subsequent records using Raw SQL 
            // This is much faster and bypasses EF Primary Key tracking issues
            string sql = @"
        UPDATE Tickets 
        SET TicketId = 'CCTNS' + CAST((CAST(REPLACE(TicketId, 'CCTNS', '') AS BIGINT) - 1) AS VARCHAR)
        WHERE CAST(REPLACE(TicketId, 'CCTNS', '') AS BIGINT) > {0}";

            await _context.Database.ExecuteSqlRawAsync(sql, deletedNum);

            TempData["SuccessMessage"] = "Ticket deleted and sequence updated successfully.";

            if (User.IsInRole("Admin")) return RedirectToAction("Index", "Tickets");
            return RedirectToAction("UserDashboard");
        }




        // Generate Ticket ID
        private async Task<string> GenerateTicketIdAsync()
        {
            int year = DateTime.Now.Year % 100;
            int month = DateTime.Now.Month;
            int count = await _context.Tickets.CountAsync(t => t.CreatedAt.Year == DateTime.Now.Year && t.CreatedAt.Month == DateTime.Now.Month) + 1;
            // Format: CCTNS + YY + MM + ### (CCTNS2510001)
            string ticketId = $"CCTNS{year:D2}{month:D2}{count:D3}";
            return ticketId;
        }

        /*        private async Task<string> GenerateTicketIdAsync(string stationCode)
                {
                    // 1. Format Prefix: CCTNS + YY + MM + STATIONCODE (e.g., CCTNS2603PS01)
                    int year = DateTime.Now.Year % 100;
                    int month = DateTime.Now.Month;
                    string prefix = $"CCTNS{year:D2}{month:D2}{stationCode}";

                    // 2. Find the highest existing TicketId for THIS specific station prefix
                    var lastTicket = await _context.Tickets
                        .Where(t => t.TicketId.StartsWith(prefix))
                        .OrderByDescending(t => t.TicketId)
                        .Select(t => t.TicketId)
                        .FirstOrDefaultAsync();

                    int nextNumber = 1;

                    if (lastTicket != null)
                    {
                        // 3. Extract the last 3 digits
                        string suffix = lastTicket.Substring(prefix.Length);
                        if (int.TryParse(suffix, out int lastSeq))
                        {
                            nextNumber = lastSeq + 1;
                        }
                    }

                    // 4. Returns station-specific sequence: CCTNS2603PS01001
                    return $"{prefix}{nextNumber:D3}";
                }*/


        private async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return uniqueFileName;
        }

        private List<SelectListItem> GetIssues()
        {
            return new List<SelectListItem>
    {
        new("POCSO", "POCSO"),

        // ===== HIGH =====
        new("Server Installation", "Server Installation"),
        new("Restore DB Backup", "Restore DB Backup"),
        new("Chargesheet Not Able to Submit", "Chargesheet Not Able to Submit"),
        new("FIR Not Able to Submit", "FIR Not Able to Submit"),
        new("Competent Authority Work Task 1", "Competent Authority Work Task 1"),
        new("Competent Authority Work Task 2", "Competent Authority Work Task 2"),
        new("Reassigning Cases", "Reassigning Cases"),
        new("Arrest Memo Not Able to Submit", "Arrest Memo Not Able to Submit"),

        // ===== MEDIUM =====
        new("CCTNS Testing Envirnoment", "CCTNS Testing Envirnoment"),
        new("CCTNS Reports Dashboard", "CCTNS Reports Dashboard"),
        new("Modification of Sections", "Modification of Sections"),
        new("Client Installation", "Client Installation"),
        new("Offline Login Issue", "Offline Login Issue"),
        new("Online Not Working", "Online Not Working"),
        new("Update Witness Address", "Update Witness Address"),
        new("Update Complainant Address", "Update Complainant Address"),
        new("Update Accused Age", "Update Accused Age"),
        new("GD Entry", "GD Entry"),
        new("GD Date & Time", "GD Date & Time"),
        new("Sync / Synchronization", "Sync / Synchronization"),
        new("DGsP & IGsP Dashboard", "DGsP & IGsP Dashboard"),
        new("Crime Details Form", "Crime Details Form"),
        new("Arrest Memo Unable to Add Witness", "Arrest Memo Unable to Add Witness"),
        new("Property Seizure Not Able to Submit", "Property Seizure Not Able to Submit"),
        new("Final Form", "Final Form"),
        new("Village Crime Details Form", "Village Crime Details Form"),
        new("Add Act and Section", "Add Act and Section"),
        new("BNS Section Not Available", "BNS Section Not Available"),
        new("SHO Issue", "SHO Issue"),

        // ===== LOW =====
        new("New User Creation", "New User Creation"),
        new("Modify User Details", "Modify User Details"),
        new("Assign IO & EO", "Assign IO & EO"),
        new("Others", "Others")
    };
        }


        private string GetPriorityByIssue(string issue)
        {
            if (string.IsNullOrWhiteSpace(issue)) return "Low";

            // Trim and Upper to handle whitespace and case-sensitivity issues
            string cleanIssue = issue.Trim().ToUpperInvariant();

            // 1. URGENT
            var urgent = new HashSet<string> { "POCSO" };

            // 2. HIGH
            var high = new HashSet<string>
    {
        "SERVER INSTALLATION", "RESTORE DB BACKUP", "CHARGESHEET NOT ABLE TO SUBMIT",
        "FIR NOT ABLE TO SUBMIT", "COMPETENT AUTHORITY WORK TASK 1",
        "COMPETENT AUTHORITY WORK TASK 2", "REASSIGNING CASES", "ARREST MEMO NOT ABLE TO SUBMIT"
    };

            // 3. MEDIUM
            var medium = new HashSet<string>
    {
       "CCTNS TESTING ENVIRNOMENT", "CCTNS REPORTS DASHBOARD", "MODIFICATION OF SECTIONS", "CLIENT INSTALLATION", "OFFLINE LOGIN ISSUE",
        "ONLINE NOT WORKING", "UPDATE WITNESS ADDRESS", "UPDATE COMPLAINANT ADDRESS",
        "UPDATE ACCUSED AGE", "GD ENTRY", "GD DATE & TIME", "SYNC / SYNCHRONIZATION",
        "DGSP & IGSP DASHBOARD", "CRIME DETAILS FORM", "ARREST MEMO UNABLE TO ADD WITNESS",
        "PROPERTY SEIZURE NOT ABLE TO SUBMIT", "FINAL FORM", "VILLAGE CRIME DETAILS FORM",
        "ADD ACT AND SECTION", "BNS SECTION NOT AVAILABLE", "SHO ISSUE"
    };

            // Logic Check
            if (urgent.Contains(cleanIssue)) return "Urgent";
            if (high.Contains(cleanIssue)) return "High";
            if (medium.Contains(cleanIssue)) return "Medium";

            return "Low";
        }


        /*   private List<SelectListItem> GetIssues()
           {
               return new List<SelectListItem>
           {
              new("POCSO", "POCSO"),                                // URGENT

   // ===== HIGH =====
   new("SERVER INSTALLATION","SERVER INSTALLATION"),
   new("RESTORE DB BACKUP","RESTORE DB BACKUP"),
   new("CHARGESHEET NOT ABLE TO SUBMIT", "CHARGESHEET NOT ABLE TO SUBMIT"),
   new("FIR NOT ABLE TO SUBMIT","FIR NOT ABLE TO SUBMIT"),
   new("COMPETENT AUTHORITY WORK TASK 2","COMPETENT AUTHORITY WORK TASK 2"),
   new("COMPETENT AUTHORITY WORK TASK 1","COMPETENT AUTHORITY WORK TASK 1"),
   new("REASSIGNING CASES", "REASSIGNING CASES"),
   new("ARREST MEMO NOT ABLE TO SUBMIT", "ARREST MEMO NOT ABLE TO SUBMIT"),

   // ===== MEDIUM =====

   new("MODIFICATION OF SECTIONS", "MODIFICATION OF SECTIONS"),
   new("CLIENT INSTALLATION", "CLIENT INSTALLATION"),
   new("OFFLINE LOGIN ISSUE", "OFFLINE LOGIN ISSUE"),
   new("ONLINE NOT WORKING", "ONLINE NOT WORKING"),
   new("UPDATE WITNESS ADDRESS", "UPDATE WITNESS ADDRESS"),
   new("UPDATE COMPLAINANT ADDRESS", "UPDATE COMPLAINANT ADDRESS"),
   new("UPDATE ACCUSED AGE", "UPDATE ACCUSED AGE"),
   new("GD ENTRY","GD ENTRY"),
   new("GD DATE & TIME","GD DATE & TIME"),
   new("SYNC","SYNC"),
   new("DGsP & IGsP DASHBOARD","DGsP & IGsP DASHBOARD"),
   new("CRIME DETAILS FORM","CRIME DETAILS FORM"),
   new("ARREST MEMO UNABLE TO ADD WITNESS","ARREST MEMO UNABLE TO ADD WITNESS"),
   new("PROPERTY SEIZURE NOT ABLE TO SUBMIT", "PROPERTY SEIZURE NOT ABLE TO SUBMIT"),
   new("FINAL FORM", "FINAL FORM"),
   new("VILLAGE CRIME DETAILS FORM", "VILLAGE CRIME DETAIL FORM"),
   new("ADD ACT AND SECTION", "ADD ACT AND SECTION"),
   new("BNS SECTION NOT AVAILABLE", "BNS SECTION NOT AVAILABLE"),
   new("SHO ISSUE", "SHO ISSUE"),
   new("ACCUSED AGE CHANGE","ACCUSED AGE CHANGE"),

   // ===== LOW =====
   new("NEW USER CREATION", "NEW USER CREATION"),
   new("MODIFY USER DETAILS", "MODIFY USER DETAILS"),

   new("OTHERS","OTHERS")
           };
           }

           private string GetPriorityByIssue(string issue)
           {
               string[] urgent = { "POCSO" };
               string[] high = {
                "Server Installation", "Restore DB Backup", "FIR not able to submit","Competent authority work task 2", "Competent authority work task 1",
                "Reassigning case","Arrest Memo not able to sumbit"
            };
               string[] medium = {
                 "Section Modification", "Client Installation", "Offline Login Issuse", "Online Login Issuse",
                "Update Witness Address", "Update Accussed Address", "Update Accussed Age", "GD Entry", "DGsP & IGsP DASHBOARD",
                "Synchronization", "Crime Detail Form", "Arrest Memo unable to add witness", "Property Seizure", "Final Form", "Village Crime Detail Form",
                "Add Act and Section", "BNS Section not available", "SHO Issuse", "GD Date & Time"
            };
               string[] low = {
                "New User Creation", "Assign IO & EO", "OTHERS"
            };

               issue = issue?.ToUpperInvariant() ?? "";

               if (urgent.Contains(issue)) return "Urgent";
               else if (high.Contains(issue)) return "High";
               else if (medium.Contains(issue)) return "Medium";
               else return "Low";
           }
   */

        /*        private string GetPriorityByIssue(string issue)
                {
                    if (string.IsNullOrWhiteSpace(issue))
                        return "Low";

                    issue = issue.Trim().ToUpperInvariant();

                    if (issue.Contains("POCSO"))
                        return "Urgent";

                    if (issue.Contains("SERVER") ||
                        issue.Contains("RESTORE") ||
                        issue.Contains("CHARGESHEET") ||
                        issue.Contains("FIR") ||
                        issue.Contains("COMPETENT") ||
                        issue.Contains("REASSIGN") ||
                        issue.Contains("ARREST MEMO"))
                        return "High";

                    if (issue.Contains("MODIFICATION") ||
                        issue.Contains("CLIENT") ||
                        issue.Contains("LOGIN") ||
                        issue.Contains("UPDATE") ||
                        issue.Contains("GD") ||
                        issue.Contains("SYNC") ||
                        issue.Contains("DASHBOARD") ||
                        issue.Contains("CRIME") ||
                        issue.Contains("PROPERTY") ||
                        issue.Contains("FINAL") ||
                        issue.Contains("ACT") ||
                        issue.Contains("SECTION") ||
                        issue.Contains("SHO"))
                        return "Medium";

                    return "Low";
                }*/



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a valid Excel file.";
                return RedirectToAction("Index", "Home");
            }

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var package = new OfficeOpenXml.ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        TempData["ErrorMessage"] = "No worksheet found in the uploaded Excel file.";
                        return RedirectToAction("Index", "Home");
                    }

                    int rowCount = worksheet.Dimension.Rows;
                    int ticketCount = 1;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            var dateReceived = worksheet.Cells[row, 3].GetValue<DateTime>();
                            var timeReceived = worksheet.Cells[row, 4].GetValue<string>();
                            var psOffice = worksheet.Cells[row, 5].GetValue<string>();
                            var region = worksheet.Cells[row, 6].GetValue<string>();
                            var callerName = worksheet.Cells[row, 7].GetValue<string>();
                            var issue = worksheet.Cells[row, 8].GetValue<string>();
                            var helpDesk = worksheet.Cells[row, 9].GetValue<string>();
                            var actionTaken = worksheet.Cells[row, 10].GetValue<string>();
                            var currentStatus = worksheet.Cells[row, 11].GetValue<string>();
                            // var resolutionTime = worksheet.Cells[row, 12].GetValue<string>();

                            // Read the resolution time safely (Excel might store it as a number or DateTime)
                            var resolutionTimeValue = worksheet.Cells[row, 12].Value;
                            string resolutionTime = resolutionTimeValue?.ToString()?.Trim() ?? string.Empty;

                            // Combine date + time safely
                            DateTime createdAt = dateReceived;
                            if (TimeSpan.TryParse(timeReceived, out var timeSpan))
                                createdAt = dateReceived.Add(timeSpan);

                            // Handle CompletedTime & CompletedAt
                            DateTime? completedAt = null;
                            DateTime? completedTime = null;

                            if (!string.IsNullOrWhiteSpace(resolutionTime))
                            {
                                // Handle both TimeSpan and DateTime Excel formats
                                if (DateTime.TryParse(resolutionTime, out var resDateTime))
                                {
                                    // If Excel gave full datetime (e.g., 01-01-2025 10:47)
                                    completedAt = createdAt.Date.Add(resDateTime.TimeOfDay);
                                    completedTime = createdAt.Date.Add(resDateTime.TimeOfDay);
                                }
                                else if (TimeSpan.TryParse(resolutionTime, out var resTime))
                                {
                                    // If Excel gave only time text (e.g., "10:47")
                                    completedAt = createdAt.Date.Add(resTime);
                                    completedTime = createdAt.Date.Add(resTime);
                                }
                            }




                            // Generate Ticket
                            var ticket = new Ticket
                            {

                                TicketId = GenerateExcelTicketId(createdAt, ticketCount),
                                Issue = issue ?? "N/A",
                                Description = actionTaken ?? " ",
                                ReporterName = callerName ?? " ",
                                PoliceStationName = psOffice ?? " ",
                                Priority = GetPriorityByIssue(issue),
                                Status = string.IsNullOrWhiteSpace(currentStatus) ? "Open" : currentStatus,
                                CreatedAt = createdAt,
                                CreatedDate = createdAt.Date,
                                CompletedAt = completedAt,
                                CompletedTime = completedTime,
                                CompletedBy = helpDesk,
                                Region = region ?? "Unknown",
                                UserId = _userManager.GetUserId(User)
                            };



                            ticketCount++;
                            var station = await _context.PoliceStations
                                .FirstOrDefaultAsync(p => p.StationName.ToLower().Contains(psOffice.ToLower()));

                            if (station != null)
                            {
                                ticket.PoliceStationCode = station.StationCode;
                                ticket.PoliceStationId = station.Id;
                            }
                            else
                            {
                                // fallback if station not found
                                ticket.PoliceStationCode = "N";
                            }

                            _context.Tickets.Add(ticket);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Row {row} import error: {ex.Message}");
                            continue;
                        }
                    }

                    await _context.SaveChangesAsync();
                }
            }

            TempData["SuccessMessage"] = "Tickets imported successfully!";
            return RedirectToAction("Index", "Home");
        }

        private string GenerateExcelTicketId(DateTime createdAt, int ticketCount)
        {
            // Increment the counter


            int year = createdAt.Year % 100; // e.g., 2025 -> 25
            int month = createdAt.Month;     // e.g., 10 -> October

            // Format: CCTNS + YY + MM + ### (CCTNS2510001)
            string ticketId = $"CCTNS{year:D2}{month:D2}{ticketCount:D3}";

            return ticketId;
        }


        public IActionResult ViewUserTicket(int id)
        {
            var ticket = _context.Tickets
                .Include(t => t.PoliceStationEntity) // only if using navigation property
                .FirstOrDefault(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }




        /*   [HttpPost]
           public async Task<IActionResult> MarkAsPending(int id, string remarks)
           {
               var ticket = await _context.Tickets.FindAsync(id);
               if (ticket == null) return NotFound();

               ticket.Status = "Pending from Police Station";
               ticket.AdminRemarks = remarks;

               await _context.SaveChangesAsync();
               return Ok(); // This tells the JavaScript "Success!"
           }*/

        [HttpPost]
        public IActionResult MarkAsPending(int id, string remarks)
        {
            var ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null) return NotFound("Ticket not found");

            ticket.AdminRemarks = remarks;

            // FIX: Change "Pending" to the full string so the dashboard recognizes it
            ticket.Status = "Pending from Police Station";

            _context.SaveChanges();
            return Ok();
        }

        /*        [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> PoliceReply(int ticketId, IFormFile ImageFile, List<IFormFile> ApprovalDocuments) // Use ApprovalDocuments here
                {
                    var ticket = await _context.Tickets.FindAsync(ticketId);
                    if (ticket == null) return NotFound();

                    // ... your file saving logic ...

                    // ADD THESE TWO LINES BEFORE SAVING
                    ticket.Status = "Open";
                    ticket.AdminRemarks = "Replied by Station on " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                    _context.Tickets.Update(ticket);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("UserDashboard", "Home");
                }*/



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PoliceReply(int ticketId, IFormFile ImageFile, List<IFormFile> ApprovalDocuments)
        {
            // 1. Fetch the existing record
            var existingTicket = await _context.Tickets.FindAsync(ticketId);
            if (existingTicket == null)
            {
                TempData["ErrorMessage"] = "Ticket record not found.";
                return RedirectToAction("Index", "UserDashboard");
            }

            string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

           // 1. Handle Screenshot (ImageFile)
    if (ImageFile != null && ImageFile.Length > 0)
    {
        // Check if it's actually an image
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(ImageFile.FileName).ToLower();

        if (allowedExtensions.Contains(extension))
        {
            string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(ImageFile.FileName)}";
            string filePath = Path.Combine(uploadFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(stream);
            }

            // Save the relative path for the <img> tag in the View
            existingTicket.ImagePath = "/uploads/" + uniqueFileName; 
        }
    }

            // 3. Handle Documents Update (Appending to existing list)
            if (ApprovalDocuments != null && ApprovalDocuments.Count > 0)
            {
                List<string> newFilePaths = new List<string>();
                foreach (var file in ApprovalDocuments)
                {
                    if (file.Length > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                        string filePath = Path.Combine(uploadFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        newFilePaths.Add("~/uploads/" + uniqueFileName);
                    }
                }

                // Logic: Add new files to the existing comma-separated string
                string existingDocs = existingTicket.ApprovalDocuments ?? "";
                string newDocsString = string.Join(",", newFilePaths);

                existingTicket.ApprovalDocuments = string.IsNullOrWhiteSpace(existingDocs)
                    ? newDocsString
                    : $"{existingDocs},{newDocsString}";
            }

            // 4. Update Status and Timestamp
            // The migration is applied, so we can now safely use UpdatedAt
            existingTicket.Status = "Open";
            existingTicket.UpdatedAt = DateTime.Now;

            try
            {
                _context.Tickets.Update(existingTicket);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Documentation submitted successfully. Ticket status is now Open.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "A database error occurred while saving your reply.";
            }

            return RedirectToAction("Index", "UserDashboard");
        }


        [HttpPost]
        public async Task<IActionResult> AdminReply(int id, string remarks)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            // ONLY save the remarks. Do not add "Feedback" text here.
            ticket.AdminRemarks = remarks;

            _context.Update(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetStationDetails(string code)
        {
            if (string.IsNullOrEmpty(code)) return BadRequest("Invalid Station Code");

            var station = await _context.PoliceStations
                .FirstOrDefaultAsync(s => s.StationCode == code);

            if (station == null) return NotFound();

            return Json(new
            {
                stationName = station.StationName,
                region = station.Region,
                stationCode = station.StationCode
            });
        }



        [HttpGet]
        public async Task<IActionResult> ExportTicketsToPdf(string policeStation, string issue, DateTime? from, DateTime? to)
        {
            var query = _context.Tickets.AsQueryable();

            // ... (Your existing filtering logic) ...
            if (!string.IsNullOrEmpty(policeStation))
                query = query.Where(t => t.PoliceStationName.Contains(policeStation));
            if (!string.IsNullOrEmpty(issue))
                query = query.Where(t => t.Issue.Contains(issue));
            if (from.HasValue)
                query = query.Where(t => t.CreatedAt >= from.Value);
            if (to.HasValue)
                query = query.Where(t => t.CreatedAt <= to.Value.Date.AddDays(1).AddTicks(-1));

            var tickets = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();

            var sb = new StringBuilder();
            sb.Append("<html><head>");
            // FIX 1: Add UTF-8 Charset to prevent weird symbols
            sb.Append("<meta charset='utf-8'>");
            sb.Append("<style>");
            sb.Append("table { width: 100%; border-collapse: collapse; font-family: sans-serif; font-size: 12px; }");
            sb.Append("th, td { border: 1px solid black; padding: 8px; text-align: left; }");
            sb.Append("th { background-color: #f2f2f2; } h2 { text-align: center; }");
            sb.Append("</style></head>");

            sb.Append("<body onload='window.print()'>");
            sb.Append("<h2>Ticket Export Report</h2>");

            sb.Append("<table><thead><tr><th>Ticket ID</th><th>Station</th><th>Issue</th><th>Status</th><th>Created Date</th><th>Completed Time</th></tr></thead><tbody>");

            foreach (var t in tickets)
            {
                // FIX 2: Use a standard keyboard hyphen "-" instead of the long dash "—"
                string completedDisplay = t.CompletedAt.HasValue
                    ? t.CompletedAt.Value.ToString("dd-MM-yyyy HH:mm")
                    : "-";

                sb.Append($"<tr><td>{t.TicketId}</td><td>{t.PoliceStationName}</td><td>{t.Issue}</td><td>{t.Status}</td><td>{t.CreatedAt:dd-MM-yyyy}</td><td>{completedDisplay}</td></tr>");
            }

            sb.Append("</tbody></table></body></html>");

            return Content(sb.ToString(), "text/html");
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmResolution(int id, string choice, string userRemarks)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            if (choice == "Yes")
            {
                ticket.UserConfirmation = "Yes";
                ticket.UserFeedback = userRemarks;
                ticket.Status = "Completed";
            }
            else if (choice == "No")
            {
                // 🔥 THE RESET: Clear confirmation so the cycle can restart
                ticket.Status = "Re-Opened";
                ticket.UserConfirmation = null; // 👈 Wipes "No" so the button reappears later
                ticket.CompletedAt = null;      // Clears date so it shows "---"
                ticket.CompletedTime = null;    // Clears time

                ticket.UserFeedback = userRemarks;
                ticket.AdminRemarks += $"\n[System] Rejected on {DateTime.Now:g}. Reason: {userRemarks}";
            }

            await _context.SaveChangesAsync();

            // ✅ Returns to your specific dashboard URL
            return Redirect("/tickets/UserDashboard");
        }



        [HttpPost]
        public async Task<IActionResult> ReOpenTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                ticket.Status = "Re-Opened";

                // 🔥 This is the fix: Clear the string so the Admin button disappears
                ticket.UserFeedback = null;

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

    }
}