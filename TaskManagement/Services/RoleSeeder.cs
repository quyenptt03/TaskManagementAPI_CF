using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManagement.Services
{
    public static class RoleSeeder
    {
        //public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<int>> roleManager)
        {
            //var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            //var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole<int> { Name = "Admin" });
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole<int> { Name = "User" });
            }
            //if (!userManager.Users.Any())
            //{
            //    var adminUser = new IdentityUser
            //    {
            //        UserName = "admin",
            //        Email = "admin@gmail.com",
            //        EmailConfirmed = true
            //    };

            //    string adminPassword = "1230123";
            //    var result = await userManager.CreateAsync(adminUser, adminPassword);

            //    if (result.Succeeded)
            //    {
            //        await userManager.AddToRoleAsync(adminUser, "Admin");
            //    }
            //}

        }


    }
}
