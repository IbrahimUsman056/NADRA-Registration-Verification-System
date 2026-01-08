using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NADRASystem.Models
{
    public class Citizen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CitizenId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Required]
        [MaxLength(15)]
        [RegularExpression(@"^\d{5}-\d{7}-\d{1}$", ErrorMessage = "CNIC must be in format: 12345-1234567-1")]
        public string? CNIC { get; set; }

        [Required]
        [MaxLength(100)]
        public string? FatherName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(10)]
        public string? Gender { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        [MaxLength(20)]
        public string? MaritalStatus { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nationality { get; set; } = "Pakistani";

        [Required]
        public bool IsAlive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property for update requests
        public virtual ICollection<CitizenUpdateRequest>? UpdateRequests { get; set; }
    }
}