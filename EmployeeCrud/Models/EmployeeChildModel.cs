using System.ComponentModel.DataAnnotations;

namespace EmployeeCrud.Models;

public class EmployeeChild
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    [Required(ErrorMessage = "نام فرزند الزامی است")]
    [Display(Name = "نام فرزند")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "تاریخ تولد")]
    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }

    [Display(Name = "جنسیت")]
    public string? Gender { get; set; }
}