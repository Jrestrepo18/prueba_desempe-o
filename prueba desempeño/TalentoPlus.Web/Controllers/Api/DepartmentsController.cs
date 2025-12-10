using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Web.Data;
using TalentoPlus.Web.Models.Dtos;

namespace TalentoPlus.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public DepartmentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// List all departments (public)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<DepartmentDto>>> ListDepartments()
        {
            try
            {
                var departments = await _db.Departments
                    .Select(d => new DepartmentDto
                    {
                        Id = d.Id,
                        Name = d.Name ?? string.Empty,
                        Description = d.Description ?? string.Empty
                    })
                    .ToListAsync();

                return Ok(departments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving departments: {ex.Message}" });
            }
        }
    }
}
