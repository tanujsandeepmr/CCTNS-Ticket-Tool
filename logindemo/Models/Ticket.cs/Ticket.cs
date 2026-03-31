using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic; // Added for List<>
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace logindemo.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string TicketId { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PoliceStationCode { get; set; } = string.Empty;

        [MaxLength(150)]
        public string PoliceStationName { get; set; } = string.Empty;

        [Required]
        public string Issue { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Priority { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Open";

        // --- IMAGE HANDLING ---
        [MaxLength(500)]
        public string? ImagePath { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; } // Used for the single Image upload

        // --- USER & STATION RELATIONSHIPS ---
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public int? PoliceStationId { get; set; }
        [ForeignKey(nameof(PoliceStationId))]
        public virtual PoliceStation? PoliceStationEntity { get; set; }

        // --- AUDIT INFO ---
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Today;
        public DateTime? CompletedAt { get; set; }
        public DateTime? CompletedTime { get; set; }
        public string? CompletedBy { get; set; }

        [Required]
        [MaxLength(100)]
        public string ReporterName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string IssueType { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Region { get; set; } = string.Empty;

        // --- MULTIPLE DOCUMENT HANDLING ---
        [MaxLength(2000)]
        public string? ApprovalDocuments { get; set; } // This stores the paths: "/uploads/1.pdf,/uploads/2.pdf"

        [NotMapped]
        public List<IFormFile>? ApprovalFiles { get; set; } // Matches the 'multiple' input in your form

        public string? PdfPath { get; set; }
        public string? AdminRemarks { get; set; }

        public string? UserConfirmation { get; set; } // New
        public string? UserFeedback { get; set; }     // New

        public DateTime? UpdatedAt { get; set; }
    }
}

























/*using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace logindemo.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string TicketId { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PoliceStationCode { get; set; } = string.Empty;

        [MaxLength(150)]
        public string PoliceStationName { get; set; } = string.Empty;

        [Required]
        public string Issue { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Priority { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Open";

        [MaxLength(500)]
        public string? ImagePath { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        // ✅ This is the foreign key to ApplicationUser
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }


        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime CreatedDate { get; set; } = DateTime.Today;

        public DateTime? CompletedAt { get; set; }

        public DateTime? CompletedTime { get; set; }

        public int? PoliceStationId { get; set; }

        [ForeignKey(nameof(PoliceStationId))]
        public virtual PoliceStation? PoliceStationEntity { get; set; }

        [Required]
        [MaxLength(100)]
        public string ReporterName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string IssueType { get; set; } = string.Empty;

        public string? CompletedBy { get; set; }

        [MaxLength(50)]
        public string Region { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? ApprovalDocuments { get; set; }

        public string? PdfPath { get; set; }


        [NotMapped]
        public List<IFormFile>? ApprovalFiles { get; set; } // for multiple uploads



        public string? AdminRemarks { get; set; }



    }
}
*/