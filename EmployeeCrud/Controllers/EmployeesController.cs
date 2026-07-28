using EmployeeCrud.Data;
using EmployeeCrud.Models;
using EmployeeCrud.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeCrud.Controllers;

public class EmployeesController : Controller
{
    private readonly AppDbContext _context;

    public EmployeesController(AppDbContext context)
    {
        _context = context;
    }

    // =========================
    // INDEX + SEARCH
    // =========================
    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.Employees
            .Include(x => x.Children)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.PersonnelCode.Contains(search) ||
                x.FirstName.Contains(search) ||
                x.LastName.Contains(search) ||
                (x.NationalCode != null &&
                 x.NationalCode.Contains(search)));
        }

        var employees = await query
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .ToListAsync();

        ViewBag.Search = search;

        return View(employees);
    }

    // =========================
    // CREATE - GET
    // =========================
    // =========================
    // CREATE - GET
    // =========================
    // =========================
    // CREATE - GET
    // =========================
    [HttpGet]
    public IActionResult Create()
    {
        return View(new EmployeeCreateViewModel());
    }


    // =========================
    // CREATE - POST
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var employee = new Employee
        {
            PersonnelCode = model.PersonnelCode,
            FirstName = model.FirstName,
            LastName = model.LastName,
            NationalCode = model.NationalCode,
            EmploymentDate = model.EmploymentDate,
            Experience = model.Experience,
            CompanyName = model.CompanyName,
            Description = model.Description
        };

        foreach (var child in model.Children)
        {
            if (string.IsNullOrWhiteSpace(child.Name))
                continue;

            employee.Children.Add(new EmployeeChild
            {
                Name = child.Name,
                BirthDate = child.BirthDate,
                Gender = child.Gender
            });
        }

        _context.Employees.Add(employee);

        await _context.SaveChangesAsync();

        TempData["Success"] = "اطلاعات پرسنل با موفقیت ثبت شد.";

        return RedirectToAction(nameof(Index));
    }


    // =========================
    // DETAILS
    // =========================
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var employee = await _context.Employees
            .Include(x => x.Children)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (employee == null)
            return NotFound();

        return View(employee);
    }


    // =========================
    // EDIT - GET
    // =========================
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var employee = await _context.Employees
            .Include(x => x.Children)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (employee == null)
            return NotFound();

        var model = new EmployeeCreateViewModel
        {
            Id = employee.Id,

            PersonnelCode = employee.PersonnelCode,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            NationalCode = employee.NationalCode,
            EmploymentDate = employee.EmploymentDate,
            Experience = employee.Experience,
            CompanyName = employee.CompanyName,
            Description = employee.Description,

            Children = employee.Children
                .Select(x => new EmployeeChildViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    BirthDate = x.BirthDate,
                    Gender = x.Gender
                })
                .ToList()
        };

        return View(model);
    }


    // =========================
    // EDIT - POST
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        EmployeeCreateViewModel model)
    {
        if (id != model.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(model);

        var employee = await _context.Employees
            .Include(x => x.Children)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (employee == null)
            return NotFound();


        // اطلاعات اصلی
        employee.PersonnelCode = model.PersonnelCode;
        employee.FirstName = model.FirstName;
        employee.LastName = model.LastName;
        employee.NationalCode = model.NationalCode;
        employee.EmploymentDate = model.EmploymentDate;
        employee.Experience = model.Experience;
        employee.CompanyName = model.CompanyName;
        employee.Description = model.Description;


        // Child های ارسال شده
        var submittedChildIds = model.Children
            .Where(x => x.Id > 0)
            .Select(x => x.Id)
            .ToHashSet();


        // Child هایی که از فرم حذف شده‌اند
        var deletedChildren = employee.Children
            .Where(x => !submittedChildIds.Contains(x.Id))
            .ToList();

        foreach (var child in deletedChildren)
        {
            _context.EmployeeChildren.Remove(child);
        }


        // Child های موجود و جدید
        foreach (var childModel in model.Children)
        {
            if (string.IsNullOrWhiteSpace(childModel.Name))
                continue;

            // ویرایش Child موجود
            if (childModel.Id > 0)
            {
                var child = employee.Children
                    .FirstOrDefault(x => x.Id == childModel.Id);

                if (child != null)
                {
                    child.Name = childModel.Name;
                    child.BirthDate = childModel.BirthDate;
                    child.Gender = childModel.Gender;
                }
            }
            // Child جدید
            else
            {
                employee.Children.Add(new EmployeeChild
                {
                    Name = childModel.Name,
                    BirthDate = childModel.BirthDate,
                    Gender = childModel.Gender
                });
            }
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "اطلاعات با موفقیت ویرایش شد.";

        return RedirectToAction(nameof(Index));
    }


    // =========================
    // DELETE - GET
    // =========================
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var employee = await _context.Employees
            .Include(x => x.Children)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (employee == null)
            return NotFound();

        return View(employee);
    }


    // =========================
    // DELETE - POST
    // =========================
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(x => x.Id == id);

        if (employee == null)
            return NotFound();

        _context.Employees.Remove(employee);

        await _context.SaveChangesAsync();

        TempData["Success"] = "پرسنل با موفقیت حذف شد.";

        return RedirectToAction(nameof(Index));
    }
}