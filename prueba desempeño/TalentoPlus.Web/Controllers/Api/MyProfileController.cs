using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalentoPlus.Web.Data;
using TalentoPlus.Web.Models.Dtos;
using TalentoPlus.Web.Services;

namespace TalentoPlus.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MyProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly PdfService _pdfService;

        public MyProfileController(ApplicationDbContext db, PdfService pdfService)
        {
            _db = db;
            _pdfService = pdfService;
        }

        /// <summary>
        /// Get information of the authenticated employee (by JWT)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<EmployeeDto>> GetProfile()
        {
            try
            {
                // Get employee ID from JWT
                var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (employeeIdClaim == null || !int.TryParse(employeeIdClaim.Value, out var employeeId))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var employee = await _db.Employees
                    .Include(e => e.Department)
                    .FirstOrDefaultAsync(e => e.Id == employeeId);

                if (employee == null)
                {
                    return NotFound(new { message = "Employee not found" });
                }

                var dto = new EmployeeDto
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName ?? string.Empty,
                    LastName = employee.LastName ?? string.Empty,
                    Email = employee.Email ?? string.Empty,
                    Position = employee.Position ?? string.Empty,
                    Salary = employee.Salary,
                    HireDate = employee.HireDate,
                    Status = employee.Status ?? string.Empty,
                    EducationLevel = employee.EducationLevel ?? string.Empty,
                    DepartmentId = employee.DepartmentId ?? 0
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving profile: {ex.Message}" });
            }
        }

        /// <summary>
        /// Download Curriculum Vitae PDF of authenticated employee
        /// </summary>
        [HttpGet("download-pdf")]
        public async Task<IActionResult> DownloadPDF()
        {
            try
            {
                // Get employee ID from JWT
                var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (employeeIdClaim == null || !int.TryParse(employeeIdClaim.Value, out var employeeId))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var employee = await _db.Employees
                    .Include(e => e.Department)
                    .FirstOrDefaultAsync(e => e.Id == employeeId);

                if (employee == null)
                {
                    return NotFound(new { message = "Employee not found" });
                }

                // Generate PDF
                var pdfBytes = _pdfService.GenerarHojaVidaPDF(employee);

                return File(pdfBytes, "application/pdf", $"CV_{employee.FirstName}_{employee.LastName}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error generating PDF: {ex.Message}" });
            }
        }
    }
}
