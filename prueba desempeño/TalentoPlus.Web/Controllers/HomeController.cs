using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Web.Data;
using TalentoPlus.Web.Models;
using TalentoPlus.Web.Models.Dtos;

namespace TalentoPlus.Web.Controllers
{
    // [Authorize(Roles = "Admin")] - DESACTIVADO EN DESARROLLO
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // Redirigir al login si el usuario no está autenticado
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            // Dashboard with statistics
            // Fetch all employees' status to memory and apply filtering there (client-side evaluation)
            var allEmployees = await _db.Employees
                .Select(e => new { e.Status })
                .ToListAsync();

            var totalEmployees = allEmployees.Count;

            // Count active employees
            var activeEmployees = allEmployees.Count(e =>
                !string.IsNullOrEmpty(e.Status) && 
                e.Status.Equals("Activo", StringComparison.OrdinalIgnoreCase));

            // Count employees on vacation
            var employeesOnVacation = allEmployees.Count(e =>
                !string.IsNullOrEmpty(e.Status) && 
                e.Status.Equals("Vacaciones", StringComparison.OrdinalIgnoreCase));

            // Count inactive employees
            var inactiveEmployees = allEmployees.Count(e =>
                !string.IsNullOrEmpty(e.Status) && 
                e.Status.Equals("Inactivo", StringComparison.OrdinalIgnoreCase));

            var dashboardData = new Dictionary<string, object>
            {
                { "TotalEmployees", totalEmployees },
                { "ActiveEmployees", activeEmployees },
                { "EmployeesOnVacation", employeesOnVacation },
                { "InactiveEmployees", inactiveEmployees }
            };

            ViewBag.Dashboard = dashboardData;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
