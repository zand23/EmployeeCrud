using EmployeeCrud.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeCrud.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.AppUsers.AnyAsync())
            return;


        var user = new AppUser
        {
            Username = "admin",
            DisplayName = "مدیر سیستم",
            IsActive = true
        };


        var hasher = new PasswordHasher<AppUser>();

        user.PasswordHash = hasher.HashPassword(
            user,
            "123456");


        context.AppUsers.Add(user);

        await context.SaveChangesAsync();
    }
}