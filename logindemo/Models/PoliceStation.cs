using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace logindemo.Models
{
    public class PoliceStation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Police Station Name")]
        public string StationName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; } = string.Empty;

        [Display(Name = "Station Code")]
        public string? StationCode { get; set; }

        [Display(Name = "Region")]
        public string? Region { get; set; }

        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Display(Name = "Contact Number")]
        public string? ContactNumber { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Display(Name = "IP Address")]
        public string? IPAddress { get; set; }

        [Display(Name = "Incharge")]
        public string? Incharge { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}