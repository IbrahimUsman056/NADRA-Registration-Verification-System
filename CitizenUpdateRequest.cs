using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NADRASystem.Models
{
    public class CitizenUpdateRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }

        [Required]
        [ForeignKey("Citizen")]
        public int CitizenId { get; set; }

        [Required]
        [ForeignKey("Department")]
        public int RequestedByDepartmentId { get; set; }

        [Required]
        [MaxLength(50)]
        public string? RequestedField { get; set; }

        public string? OldValue { get; set; }

        [Required]
        public string? NewValue { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        public DateTime RequestedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Citizen? Citizen { get; set; }
        public virtual Department? Department { get; set; }
    }
}