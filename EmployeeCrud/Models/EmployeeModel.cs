using System.ComponentModel.DataAnnotations;

namespace EmployeeCrud.Models;

public class Employee
{
    public int Id { get; set; }

    [Required(ErrorMessage = "کد پرسنلی الزامی است")]
    [Display(Name = "کد پرسنلی")]
    public string PersonnelCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام الزامی است")]
    [Display(Name = "نام")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام خانوادگی الزامی است")]
    [Display(Name = "نام خانوادگی")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "کد ملی")]
    public string? NationalCode { get; set; }

    [Display(Name = "تاریخ استخدام")]
    [DataType(DataType.Date)]
    public DateTime? EmploymentDate { get; set; }

    [Display(Name = "سابقه")]
    public int? Experience { get; set; }

    [Display(Name = "نام شرکت")]
    public string? CompanyName { get; set; }

    [Display(Name = "توضیحات")]
    public string? Description { get; set; }

    // User who created this employee
    public int? CreatedByUserId { get; set; }

    public AppUser? CreatedByUser { get; set; }
    public ICollection<EmployeeChild> Children { get; set; }
        = new List<EmployeeChild>();
}