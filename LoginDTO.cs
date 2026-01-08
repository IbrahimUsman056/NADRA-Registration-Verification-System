using System.ComponentModel.DataAnnotations;

namespace NADRASystem.DTOs
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

public class RegisterDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string FullName { get; set; }

    public int? DepartmentId { get; set; }

    [Required]
    public string Role { get; set; }
}