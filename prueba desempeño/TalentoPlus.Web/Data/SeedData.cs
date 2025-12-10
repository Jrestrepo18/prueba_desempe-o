using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Web.Models;

namespace TalentoPlus.Web.Data
{
    public static class SeedData
    {
        /// <summary>
        /// Initialize database with roles, departments, and admin user
        /// </summary>
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Ensure database is created
            await context.Database.MigrateAsync();

            // ===== Create Roles =====
            var roles = new[] { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // ===== Create Default Departments =====
            if (!context.Departments.Any())
            {
                var departments = new[]
                {
                    new Department { Name = "Recursos Humanos", Description = "Departamento de Recursos Humanos" },
                    new Department { Name = "Tecnología", Description = "Departamento de Tecnología e Informática" },
                    new Department { Name = "Ventas", Description = "Departamento de Ventas" },
                    new Department { Name = "Marketing", Description = "Departamento de Marketing" },
                    new Department { Name = "Finanzas", Description = "Departamento de Finanzas y Contabilidad" }
                };

                context.Departments.AddRange(departments);
                await context.SaveChangesAsync();
            }

            // ===== Create Default Admin User =====
            var adminEmail = "admin@talentoplus.local";
            var adminPassword = "Admin@123456"; // Admin password
            var adminExists = await userManager.FindByEmailAsync(adminEmail);

            if (adminExists == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    // Assign Admin role
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // ===== Create Default Admin Employee =====
            var adminEmployee = context.Employees.FirstOrDefault(e => e.Email == adminEmail);

            if (adminEmployee == null)
            {
                var hrDept = context.Departments.FirstOrDefault(d => d.Name == "Recursos Humanos");
                
                adminEmployee = new Employee
                {
                    FirstName = "Admin",
                    LastName = "System",
                    Document = "0000000000",
                    Email = adminEmail,
                    Phone = "+57 (1) 1234567",
                    Address = "Calle Principal 123, Bogotá",
                    Position = "Administrador del Sistema",
                    Salary = 5000000,
                    HireDate = DateTime.UtcNow,
                    Status = "Active",
                    EducationLevel = "Maestría",
                    BirthDate = new DateTime(1990, 1, 1),
                    DepartmentId = hrDept?.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Employees.Add(adminEmployee);
                await context.SaveChangesAsync();
            }
        }
    }
}
