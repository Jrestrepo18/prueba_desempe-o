using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalentoPlus.Web.Data;
using TalentoPlus.Web.Services;

namespace TalentoPlus.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly PdfService _pdfService;

        public ProfileController(ApplicationDbContext db, PdfService pdfService)
        {
            _db = db;
            _pdfService = pdfService;
        }

        /// <summary>
        /// Show employee's own profile
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                // Get employee ID from JWT or Identity claim
                var employeeIdClaim = User.FindFirst("EmployeeId");
                
                if (employeeIdClaim == null || !int.TryParse(employeeIdClaim.Value, out var employeeId))
                {
                    // Try with document and email from claims (self-registered users)
                    var documentClaim = User.FindFirst("Document");
                    var emailClaim = User.FindFirst(ClaimTypes.Email);
                    
                    if (documentClaim != null && emailClaim != null)
                    {
                        var employee = await _db.Employees
                            .Include(e => e.Department)
                            .FirstOrDefaultAsync(e => 
                                e.Document == documentClaim.Value && 
                                e.Email == emailClaim.Value);

                        if (employee != null)
                        {
                            return View(employee);
                        }
                    }

                    return Unauthorized();
                }

                var currentEmployee = await _db.Employees
                    .Include(e => e.Department)
                    .FirstOrDefaultAsync(e => e.Id == employeeId);

                if (currentEmployee == null)
                {
                    return NotFound();
                }

                return View(currentEmployee);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading profile: {ex.Message}");
                return View();
            }
        }

        /// <summary>
        /// Download employee's own CV as PDF
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DownloadCV()
        {
            try
            {
                var documentClaim = User.FindFirst("Document");
                var emailClaim = User.FindFirst(ClaimTypes.Email);

                if (documentClaim == null || emailClaim == null)
                {
                    return Unauthorized();
                }

                var employee = await _db.Employees.FirstOrDefaultAsync(e => 
                    e.Document == documentClaim.Value && e.Email == emailClaim.Value);

                if (employee == null)
                {
                    return NotFound();
                }

                var pdfBytes = _pdfService.GenerarHojaVidaPDF(employee);
                return File(pdfBytes, "application/pdf", $"CV_{employee.FirstName}_{employee.LastName}.pdf");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error generating PDF: {ex.Message}");
                return BadRequest();
            }
        }

        /// <summary>
        /// Edit employee's own profile information
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            try
            {
                var documentClaim = User.FindFirst("Document");
                var emailClaim = User.FindFirst(ClaimTypes.Email);

                if (documentClaim == null || emailClaim == null)
                {
                    return Unauthorized();
                }

                var employee = await _db.Employees.FirstOrDefaultAsync(e => 
                    e.Document == documentClaim.Value && e.Email == emailClaim.Value);

                if (employee == null)
                {
                    return NotFound();
                }

                return View(employee);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading profile: {ex.Message}");
                return View();
            }
        }

        /// <summary>
        /// Update employee's own profile information
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Phone,Address,BirthDate,EducationLevel,ProfessionalProfile")] Models.Employee employee)
        {
            try
            {
                // Get current employee
                var currentEmployee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == id);
                
                if (currentEmployee == null)
                {
                    return NotFound();
                }

                // Verify user owns this profile
                var documentClaim = User.FindFirst("Document");
                var emailClaim = User.FindFirst(ClaimTypes.Email);

                if (documentClaim == null || emailClaim == null || 
                    currentEmployee.Document != documentClaim.Value || 
                    currentEmployee.Email != emailClaim.Value)
                {
                    return Forbid();
                }

                if (ModelState.IsValid)
                {
                    // Only allow editing certain fields
                    currentEmployee.FirstName = employee.FirstName;
                    currentEmployee.LastName = employee.LastName;
                    currentEmployee.Phone = employee.Phone;
                    currentEmployee.Address = employee.Address;
                    currentEmployee.BirthDate = employee.BirthDate;
                    currentEmployee.EducationLevel = employee.EducationLevel;
                    currentEmployee.ProfessionalProfile = employee.ProfessionalProfile;
                    currentEmployee.UpdatedAt = DateTime.UtcNow;

                    _db.Employees.Update(currentEmployee);
                    await _db.SaveChangesAsync();

                    ViewBag.Message = "Perfil actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                return View(currentEmployee);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating profile: {ex.Message}");
                return View(employee);
            }
        }
    }
}
