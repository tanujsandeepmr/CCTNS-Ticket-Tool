using Microsoft.AspNetCore.Mvc;
using logindemo.Data;
using logindemo.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace logindemo.Controllers
{
    [Authorize(Roles = "Admin")] // ⬅ Restrict access to Admins only by default
    public class PoliceStationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PoliceStationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PoliceStation/Login
        [HttpGet]
        [AllowAnonymous] // ⬅ Allow everyone to access Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: PoliceStation/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous] // ⬅ Allow everyone to access Login
        public IActionResult Login(string stationName, string password)
        {
            if (string.IsNullOrWhiteSpace(stationName) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            string hashedPassword = HashPassword(password);
            var station = _context.PoliceStations
                .FirstOrDefault(p => p.StationName == stationName && p.PasswordHash == hashedPassword);

            if (station == null)
            {
                ViewBag.Error = "Invalid station name or password.";
                return View();
            }

            TempData["PoliceStationName"] = station.StationName;
            TempData["PoliceStationId"] = station.Id.ToString();
            return RedirectToAction("Dashboard");
        }

        // POST: PoliceStation/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous] // ⬅ Allow everyone to access Register
        public IActionResult Register(string stationName, string password)
        {
            if (string.IsNullOrWhiteSpace(stationName) || string.IsNullOrWhiteSpace(password))
            {
                TempData["RegError"] = "All fields are required.";
                return RedirectToAction("Login");
            }

            if (_context.PoliceStations.Any(p => p.StationName == stationName))
            {
                TempData["RegError"] = "A police station with this name already exists.";
                return RedirectToAction("Login");
            }

            var newStation = new PoliceStation
            {
                StationName = stationName,
                PasswordHash = HashPassword(password),
                CreatedAt = DateTime.Now
            };

            _context.PoliceStations.Add(newStation);
            _context.SaveChanges();

            TempData["RegSuccess"] = "Registration successful. Please log in.";
            return RedirectToAction("Login");
        }

        // GET: PoliceStation/Dashboard
        [HttpGet]
        public IActionResult Dashboard()
        {
            if (!TempData.TryGetValue("PoliceStationName", out var stationNameObj) || stationNameObj == null)
                return RedirectToAction("Login");

            string stationName = stationNameObj.ToString()!;
            if (string.IsNullOrEmpty(stationName))
                return RedirectToAction("Login");

            TempData.Keep();

            var station = _context.PoliceStations
                .Include(p => p.Tickets)
                .FirstOrDefault(p => p.StationName == stationName);

            if (station == null)
                return NotFound();

            return View("Dashboard", station);
        }

        // GET: PoliceStation/PoliceTable
        [HttpGet]
        public IActionResult PoliceTable()
        {
            var allStations = _context.PoliceStations.OrderBy(p => p.StationName).ToList();
            return View(allStations);
        }

        // GET: PoliceStation/ViewDetails/5
        [HttpGet]
        public IActionResult ViewDetails(int id)
        {
            var station = _context.PoliceStations.FirstOrDefault(p => p.Id == id);
            if (station == null)
                return NotFound();

            return PartialView("_PoliceStationDetailsPartial", station);
        }

        // POST: PoliceStation/UpdateDetails
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateDetails(PoliceStation model)
        {
            var station = _context.PoliceStations.FirstOrDefault(p => p.Id == model.Id);
            if (station == null)
                return NotFound();

            station.Location = model.Location;
            station.StationCode = model.StationCode;
            station.Region = model.Region;
            station.ContactNumber = model.ContactNumber;
            station.IPAddress = model.IPAddress;
            station.Incharge = model.Incharge;

            _context.PoliceStations.Update(station);
            _context.SaveChanges();

            TempData["PoliceStationName"] = station.StationName;
            TempData["UpdateSuccess"] = "Details updated successfully.";
            return RedirectToAction("PoliceTable");
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public IActionResult Edit(int id)
        {
            var station = _context.PoliceStations.FirstOrDefault(s => s.Id == id);
            if (station == null)
                return NotFound();
            return View(station);
        }

        // POST: PoliceStation/UpdateStation
        [HttpPost]
        public IActionResult UpdateStation(int id, string stationCode, string stationName, string Region, string Location, string ContactNumber, string Email, string IPAddress, string Incharge)
        {
            var station = _context.PoliceStations.FirstOrDefault(s => s.Id == id);
            if (station != null)
            {
                station.StationCode = stationCode;
                station.StationName = stationName;
                station.Region = Region;
                station.Location = Location;
                station.ContactNumber = ContactNumber;
                station.Email = Email;
                station.IPAddress = IPAddress;
                station.Incharge = Incharge;

                _context.SaveChanges();
                TempData["Message"] = "Station updated successfully.";
            }
            else
            {
                TempData["Error"] = "Station not found.";
            }

            return RedirectToAction("PoliceTable");
        }

        // GET: PoliceStation/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PoliceStation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PoliceStation station)
        {
            if (!ModelState.IsValid)
                return View(station);

            station.CreatedAt = DateTime.Now;
            _context.PoliceStations.Add(station);
            await _context.SaveChangesAsync();

            return RedirectToAction("PoliceTable");
        }
    }
}
