using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Web.Data;

namespace TalentoPlus.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Obtiene estadísticas del dashboard
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<dynamic>> GetStats()
        {
            try
            {
                var totalEmployees = await _db.Employees.CountAsync();
                var activeEmployees = await _db.Employees.CountAsync(e => e.Status == "Activo");
                var inactiveEmployees = totalEmployees - activeEmployees;

                return Ok(new 
                { 
                    success = true, 
                    totalEmployees, 
                    activeEmployees, 
                    inactiveEmployees,
                    activityRate = totalEmployees > 0 ? ((activeEmployees * 100) / totalEmployees) : 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
