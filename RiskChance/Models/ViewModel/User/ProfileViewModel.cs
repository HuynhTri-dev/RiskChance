using System.ComponentModel.DataAnnotations;

namespace RiskChance.Models
{
    public class ProfileViewModel
    {
        [Required]
        public string UserID { get; set; } = string.Empty;
        [Required]
        public string? FullName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        [Required]
        public string RoleAccount { get; set; } = string.Empty; 
    }
}
