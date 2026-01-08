using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NADRASystem.Models
{
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DepartmentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? DepartmentName { get; set; }

        [Required]
        [MaxLength(50)]
        public string? DepartmentType { get; set; }

        public bool IsActive { get; set; } = true;

        
        public virtual ICollection<ApplicationUser>? Users { get; set; }
        public virtual ICollection<CitizenUpdateRequest>? UpdateRequests { get; set; }
    }
}