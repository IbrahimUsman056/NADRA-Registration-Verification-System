using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NADRASystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string? FullName { get; set; }

        [ForeignKey("Department")]
        public int? DepartmentId { get; set; }

        // Navigation property
        public virtual Department? Department { get; set; }
    }
}