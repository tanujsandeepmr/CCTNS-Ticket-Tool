using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Need this for [Table]

namespace logindemo.Models
{
    [Table("FeedbacksCCTNS")] // This maps the class to your specific SQL table name
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        public string StationName { get; set; }

        public int CooperationRating { get; set; }

        public int MaintenanceRating { get; set; }

        public string Comments { get; set; }

        public DateTime SubmittedDate { get; set; }
    }
}