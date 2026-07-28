using System.ComponentModel.DataAnnotations;

namespace EmployeeCrud.Models;

public class AppUser
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<Employee> Employees { get; set; }
        = new List<Employee>();
}