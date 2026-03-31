using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using logindemo.Models;
using System;

namespace logindemo.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Tables
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<PoliceStation> PoliceStations { get; set; }

        public DbSet<Feedback> Feedbacks { get; set; }

        private static string NormalizeUserName(string stationName)
        {
            return stationName
                .ToUpperInvariant()
                .Replace("&", "AND")
                .Replace("(", "")
                .Replace(")", "")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("  ", " ")
                .Trim()
                .Replace(" ", "_");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map Tickets trigger to avoid OUTPUT errors
            modelBuilder.Entity<Ticket>()
                .ToTable(tb => tb.HasTrigger("trg_Tickets_SetRegion"));

            // Fixed CreatedAt for all police stations
            DateTime createdAt = new DateTime(2025, 6, 5, 8, 0, 0);

            // Seed Police Stations
            modelBuilder.Entity<PoliceStation>().HasData(
                new PoliceStation { Id = 1, StationCode = "26001001", StationName = "FOOD CELL PS KARAIKAL", CreatedAt = createdAt },
                new PoliceStation { Id = 2, StationCode = "26001002", StationName = "KOTTUCHERRY", CreatedAt = createdAt },
                new PoliceStation { Id = 3, StationCode = "26001003", StationName = "TOWN KARAIKAL", CreatedAt = createdAt },
                new PoliceStation { Id = 4, StationCode = "26001004", StationName = "NEDUNGADU", CreatedAt = createdAt },
                new PoliceStation { Id = 5, StationCode = "26001005", StationName = "NERAVY", CreatedAt = createdAt },
                new PoliceStation { Id = 6, StationCode = "26001006", StationName = "THIRUNALLAR", CreatedAt = createdAt },
                new PoliceStation { Id = 7, StationCode = "26001007", StationName = "T.R.PATTINAM", CreatedAt = createdAt },
                new PoliceStation { Id = 8, StationCode = "26001008", StationName = "WOMEN PS KARAIKAL", CreatedAt = createdAt },
                new PoliceStation { Id = 9, StationCode = "26001009", StationName = "PCR CELL PS KARAIKAL", CreatedAt = createdAt },
                new PoliceStation { Id = 10, StationCode = "26001010", StationName = "TRAFFIC NORTH KARAIKAL", CreatedAt = createdAt },
                new PoliceStation { Id = 11, StationCode = "26001011", StationName = "EXCISE PS KARAIKAL", CreatedAt = createdAt },
                new PoliceStation { Id = 12, StationCode = "26001012", StationName = "COASTAL PS KARAIKAL", CreatedAt = createdAt },
                new PoliceStation { Id = 13, StationCode = "26001013", StationName = "TRAFFIC SOUTH KARAIKAL", CreatedAt = createdAt },
                new PoliceStation { Id = 14, StationCode = "26002001", StationName = "VAC PS (VIGILANCE & ANTI CORRUPTION)", CreatedAt = createdAt },
                new PoliceStation { Id = 15, StationCode = "26002002", StationName = "CBCID PS", CreatedAt = createdAt },
                new PoliceStation { Id = 16, StationCode = "26002003", StationName = "DHANVANTRI NAGAR (D. NAGAR)", CreatedAt = createdAt },
                new PoliceStation { Id = 17, StationCode = "26002004", StationName = "REDDIARPALAYAM", CreatedAt = createdAt },
                new PoliceStation { Id = 18, StationCode = "26002005", StationName = "MUDALIARPET", CreatedAt = createdAt },
                new PoliceStation { Id = 19, StationCode = "26002006", StationName = "METTUPALAYAM", CreatedAt = createdAt },
                new PoliceStation { Id = 20, StationCode = "26002007", StationName = "VILLIANUR", CreatedAt = createdAt },
                new PoliceStation { Id = 21, StationCode = "26002008", StationName = "WOMEN PS VILLIANUR", CreatedAt = createdAt },
                new PoliceStation { Id = 22, StationCode = "26002009", StationName = "MANGALAM", CreatedAt = createdAt },
                new PoliceStation { Id = 23, StationCode = "26002010", StationName = "FOOD CELL PS PUDUCHERRY", CreatedAt = createdAt },
                new PoliceStation { Id = 24, StationCode = "26002011", StationName = "MAHE PS", CreatedAt = createdAt },
                new PoliceStation { Id = 25, StationCode = "26002012", StationName = "PALLOOR PS", CreatedAt = createdAt },
                new PoliceStation { Id = 26, StationCode = "26002013", StationName = "YANAM PS", CreatedAt = createdAt },
                new PoliceStation { Id = 27, StationCode = "26002014", StationName = "PCR CELL PS PUDUCHERRY", CreatedAt = createdAt },
                new PoliceStation { Id = 28, StationCode = "26002015", StationName = "PCR CELL PS YANAM", CreatedAt = createdAt },
                new PoliceStation { Id = 29, StationCode = "26002016", StationName = "GRAND BAZAR", CreatedAt = createdAt },
                new PoliceStation { Id = 30, StationCode = "26002017", StationName = "KALAPET", CreatedAt = createdAt },
                new PoliceStation { Id = 31, StationCode = "26002018", StationName = "MUTHIALPET", CreatedAt = createdAt },
                new PoliceStation { Id = 32, StationCode = "26002019", StationName = "LAWSPET", CreatedAt = createdAt },
                new PoliceStation { Id = 33, StationCode = "26002020", StationName = "ODIANSALAI", CreatedAt = createdAt },
                new PoliceStation { Id = 34, StationCode = "26002021", StationName = "ORLEANPET", CreatedAt = createdAt },
                new PoliceStation { Id = 35, StationCode = "26002022", StationName = "AWPS PUDUCHERRY", CreatedAt = createdAt },
                new PoliceStation { Id = 36, StationCode = "26002023", StationName = "TRAFFIC EAST PUDUCHERRY", CreatedAt = createdAt },
                new PoliceStation { Id = 37, StationCode = "26002024", StationName = "TRAFFIC WEST PUDUCHERRY", CreatedAt = createdAt },
                new PoliceStation { Id = 38, StationCode = "26002025", StationName = "ARIANKUPPAM", CreatedAt = createdAt },
                new PoliceStation { Id = 39, StationCode = "26002026", StationName = "SEDARAPET", CreatedAt = createdAt },
                new PoliceStation { Id = 40, StationCode = "26002027", StationName = "BAHOUR", CreatedAt = createdAt },
                new PoliceStation { Id = 41, StationCode = "26002028", StationName = "KIRUMAMPAKKAM", CreatedAt = createdAt },
                new PoliceStation { Id = 42, StationCode = "26002029", StationName = "KATTERIKUPPAM", CreatedAt = createdAt },
                new PoliceStation { Id = 43, StationCode = "26002030", StationName = "NETTAPAKKAM", CreatedAt = createdAt },
                new PoliceStation { Id = 44, StationCode = "26002031", StationName = "THAVALAKUPPAM", CreatedAt = createdAt },
                new PoliceStation { Id = 45, StationCode = "26002032", StationName = "THIRUBUVANAI", CreatedAt = createdAt },
                new PoliceStation { Id = 46, StationCode = "26002033", StationName = "THIRUKANUR", CreatedAt = createdAt },
                new PoliceStation { Id = 47, StationCode = "26002034", StationName = "EXCISE PS MAHE", CreatedAt = createdAt },
                new PoliceStation { Id = 48, StationCode = "26002035", StationName = "EXCISE PS YANAM", CreatedAt = createdAt },
                new PoliceStation { Id = 49, StationCode = "26002036", StationName = "TRAFFIC SOUTH PUDUCHERRY", CreatedAt = createdAt },
                new PoliceStation { Id = 50, StationCode = "26002037", StationName = "EXCISE PS PUDUCHERRY", CreatedAt = createdAt },
                new PoliceStation { Id = 51, StationCode = "26002038", StationName = "COASTAL PS PUDUCHERRY", CreatedAt = createdAt },
                new PoliceStation { Id = 52, StationCode = "26002039", StationName = "COASTAL PS MAHE", CreatedAt = createdAt },
                new PoliceStation { Id = 53, StationCode = "26002040", StationName = "COASTAL PS YANAM", CreatedAt = createdAt },
                new PoliceStation { Id = 54, StationCode = "26002041", StationName = "TRAFFIC NORTH PUDUCHERRY", CreatedAt = createdAt },
                new PoliceStation { Id = 55, StationCode = "26002042", StationName = "PS ECONOMIC OFFENSES WING", CreatedAt = createdAt },
                new PoliceStation { Id = 56, StationCode = "26002043", StationName = "PS CYBER CRIME CELL", CreatedAt = createdAt },
                new PoliceStation { Id = 57, StationCode = "26002044", StationName = "TRAFFIC YANAM POLICE STATION", CreatedAt = createdAt }
            );

        }
    }
}
