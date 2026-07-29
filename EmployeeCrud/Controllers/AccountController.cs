using System.Security.Claims;
using EmployeeCrud.Data;
using EmployeeCrud.Models;
using EmployeeCrud.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeCrud.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<AppUser> _passwordHasher;

    public AccountController(AppDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<AppUser>();
    }


    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Employees");

        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _context.AppUsers
            .FirstOrDefaultAsync(x =>
                x.Username == model.Username &&
                x.IsActive);

        if (user == null)
        {
            ModelState.AddModelError(
                "",
                "نام کاربری یا رمز عبور اشتباه است.");

            return View(model);
        }

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            model.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(
                "",
                "نام کاربری یا رمز عبور اشتباه است.");

            return View(model);
        }

        var claims = new List<Claim>
    {
        new Claim(
            ClaimTypes.NameIdentifier,
            user.Id.ToString()),

        new Claim(
            ClaimTypes.Name,
            user.Username),

        new Claim(
            "DisplayName",
            user.DisplayName ?? user.Username)
    };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            });

        return RedirectToAction("Index", "Employees");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(nameof(Login));
    }


    public IActionResult AccessDenied()
    {
        return View();
    }
}