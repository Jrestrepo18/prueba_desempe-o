using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Web.Data;
using TalentoPlus.Web.Models;

namespace TalentoPlus.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Muestra el dashboard con estadísticas e interfaz de IA
        /// Solo para Admin
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                // Calcular estadísticas
                var totalEmployees = await _db.Employees.CountAsync();
                var activeEmployees = await _db.Employees.CountAsync(e => e.Status == "Active");
                var employeesByDepartment = await _db.Employees
                    .GroupBy(e => e.DepartmentId)
                    .Select(g => new { DepartmentId = g.Key, Count = g.Count() })
                    .ToListAsync();

                // Obtener departamentos para la vista
                var departments = await _db.Departments.ToListAsync();

                ViewBag.TotalEmployees = totalEmployees;
                ViewBag.ActiveEmployees = activeEmployees;
                ViewBag.InactiveEmployees = totalEmployees - activeEmployees;
                ViewBag.Departments = departments;
                ViewBag.EmployeesByDepartment = employeesByDepartment;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar el dashboard: {ex.Message}";
                return View();
            }
        }
    }
}
