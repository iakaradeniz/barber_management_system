using barber_management_system.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

public static class SeedData
{
    public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Rolleri tanımla
        string[] roleNames = { "Admin", "Calisan", "Musteri" };

        // Her bir rolü kontrol et, yoksa oluştur
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!roleResult.Succeeded)
                {
                    throw new Exception($"{roleName} rolü oluşturulurken bir hata oluştu: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
        }

        // Admin kullanıcıları tanımla
        var adminUsers = new[]
        {
            new { Email = "admin@example.com", Password = "Admin123*" }
        };

        foreach (var admin in adminUsers)
        {
            var user = await userManager.FindByEmailAsync(admin.Email);
            if (user == null)
            {
                user = new IdentityUser { UserName = admin.Email, Email = admin.Email };
                var createResult = await userManager.CreateAsync(user, admin.Password);

                if (!createResult.Succeeded)
                {
                    throw new Exception($"Admin kullanıcısı {admin.Email} oluşturulurken bir hata oluştu: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                var addToRoleResult = await userManager.AddToRoleAsync(user, "Admin");
                if (!addToRoleResult.Succeeded)
                {
                    throw new Exception($"Admin kullanıcısı {admin.Email} rolüne eklenirken bir hata oluştu: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}