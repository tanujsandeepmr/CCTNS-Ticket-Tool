using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace logindemo.Models
{
    /// <summary>
    /// Extended Identity user with custom properties for the CCTNS system.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Full name of the user.
        /// </summary>
        [Required(ErrorMessage = "Full Name is required")]
        [MaxLength(100, ErrorMessage = "Full Name cannot exceed 100 characters")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Police Station Code of the user.
        /// </summary>
        [Required(ErrorMessage = "Police Station Code is required")]
        [MaxLength(20)]
        public string PoliceStationCode { get; set; } = string.Empty;

        /// <summary>
        /// Session Token to restrict multiple logins.
        /// </summary>
        public string? SessionToken { get; set; }
    }
}
