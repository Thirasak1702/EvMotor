using EbikeRental.Domain.Entities;
using EbikeRental.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EbikeRental.Infrastructure.Identity;

public static class IdentitySeed
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        // Create roles
        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new AppRole(role));
            }
        }

        // Create super admin user
        var superAdminEmail = "admin@ebikerental.com";
        var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);

        if (superAdmin == null)
        {
            superAdmin = new AppUser
            {
                UserName = superAdminEmail,
                Email = superAdminEmail,
                FirstName = "Super",
                LastName = "Admin",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(superAdmin, "Admin@123");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(superAdmin, Roles.SuperAdmin);
            }
        }
    }
}
