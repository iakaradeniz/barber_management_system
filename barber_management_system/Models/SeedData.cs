using barber_management_system.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace barber_management_system.Models
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Admin rolünü kontrol et, yoksa oluştur
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Admin kullanıcıları tanımla
            var adminUsers = new[]
            {
                new { Email = "admin@example.com", Password = "Admin123" }
               
            };

            foreach (var admin in adminUsers)
            {
                if (await userManager.FindByEmailAsync(admin.Email) == null)
                {
                    var adminUser = new IdentityUser { UserName = admin.Email, Email = admin.Email };
                    var result = await userManager.CreateAsync(adminUser, admin.Password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }
        }
    }
}