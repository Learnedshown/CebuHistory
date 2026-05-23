using Microsoft.AspNetCore.Identity;
using CebuHistory.Models;

namespace CebuHistory.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // Ensure roles exist
        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Seed default admin
        const string adminEmail = "admin@sugbo.ph";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Sugbo",
                LastName = "Admin",
                EmailConfirmed = true,
                IsActive = true,
            };
            var result = await userManager.CreateAsync(admin, "Admin@1234");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}