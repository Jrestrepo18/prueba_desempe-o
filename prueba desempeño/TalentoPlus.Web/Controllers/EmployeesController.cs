using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Web.Data;
using TalentoPlus.Web.Models;
using TalentoPlus.Web.Repositories;
using TalentoPlus.Web.Services;
using OfficeOpenXml;

namespace TalentoPlus.Web.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IRepositoryEmployee _repository;
        private readonly ApplicationDbContext _db;
        private readonly PdfService _pdfService;

        public EmployeesController(IRepositoryEmployee repository, ApplicationDbContext db, PdfService pdfService)
        {
            _repository = repository;
            _db = db;
            _pdfService = pdfService;
        }

        /// <summary>
        /// List all employees
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var employees = await _repository.GetAllAsync();
            return View(employees.ToList());
        }

        /// <summary>
        /// View employee details
        /// </summary>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var employee = await _repository.GetByIdAsync(id.Value);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        /// <summary>
        /// Create new employee - GET
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create new employee - POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Document,Email,Phone,Address,Position,Salary,HireDate,Status,EducationLevel,BirthDate,ProfessionalProfile,DepartmentId")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.CreatedAt = DateTime.UtcNow;
                employee.UpdatedAt = DateTime.UtcNow;
                await _repository.AddAsync(employee);
                return RedirectToAction(nameof(Index));
            }

            var departments = _db.Departments
                .Select(d => new SelectListItem 
                { 
                    Value = d.Id.ToString(), 
                    Text = d.Name 
                })
                .ToList();
            ViewBag.Departments = departments;
            return View(employee);
        }

        /// <summary>
        /// Edit employee - GET
        /// </summary>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var employee = await _repository.GetByIdAsync(id.Value);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        /// <summary>
        /// Edit employee - POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Document,Email,Phone,Address,Position,Salary,HireDate,Status,EducationLevel,BirthDate,ProfessionalProfile,Department")] Employee employee)
        {
            if (id != employee.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    employee.UpdatedAt = DateTime.UtcNow;
                    await _repository.UpdateAsync(employee);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _repository.GetByIdAsync(id) == null)
                        return NotFound();
                    throw;
                }
            }

            return View(employee);
        }

        /// <summary>
        /// Delete employee - GET
        /// </summary>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var employee = await _repository.GetByIdAsync(id.Value);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        /// <summary>
        /// Delete employee - POST (Marcar como Inactivo)
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _repository.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            // Cambiar estado a Inactivo en lugar de eliminar
            employee.Status = "Inactivo";
            employee.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(employee);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Import employees from Excel
        /// </summary>
        public IActionResult ImportExcel()
        {
            return View();
        }

        /// <summary>
        /// Process Excel file - POST
        /// </summary>
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ImportExcel(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                ModelState.AddModelError("", "Please select an Excel file");
                return View();
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await arquivo.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension?.Rows ?? 0;

                        for (int row = 2; row <= rowCount; row++) // Skip header
                        {
                            try
                            {
                                // Column mapping according to Excel structure:
                                // 1. Documento, 2. Nombre, 3. Apellidos, 4. Fecha Nacimiento
                                // 5. Dirección, 6. Teléfono, 7. Email, 8. Cargo
                                // 9. Salario, 10. Fecha Ingreso, 11. Estado, 12. Nivel Educativo
                                // 13. Perfil Profesional, 14. Departamento
                                
                                var documentCell = worksheet.Cells[row, 1].Value;
                                var firstNameCell = worksheet.Cells[row, 2].Value;
                                var lastNameCell = worksheet.Cells[row, 3].Value;
                                var birthDateCell = worksheet.Cells[row, 4].Value;
                                var addressCell = worksheet.Cells[row, 5].Value;
                                var phoneCell = worksheet.Cells[row, 6].Value;
                                var emailCell = worksheet.Cells[row, 7].Value;
                                var positionCell = worksheet.Cells[row, 8].Value;
                                var salaryCell = worksheet.Cells[row, 9].Value;
                                var hireDateCell = worksheet.Cells[row, 10].Value;
                                var statusCell = worksheet.Cells[row, 11].Value;
                                var educCell = worksheet.Cells[row, 12].Value;
                                var profileCell = worksheet.Cells[row, 13].Value;
                                var deptCell = worksheet.Cells[row, 14].Value;

                                var document = documentCell?.ToString() ?? "";
                                var firstName = firstNameCell?.ToString() ?? "";
                                var lastName = lastNameCell?.ToString() ?? "";
                                var address = addressCell?.ToString() ?? "";
                                var phone = phoneCell?.ToString() ?? "";
                                var email = emailCell?.ToString() ?? "";
                                var position = positionCell?.ToString() ?? "";
                                var salaryStr = salaryCell?.ToString() ?? "0";
                                var hireDateStr = hireDateCell?.ToString() ?? "";
                                var status = statusCell?.ToString() ?? "Activo";
                                var educationLevel = educCell?.ToString() ?? "";
                                var professionalProfile = profileCell?.ToString() ?? "";
                                var department = deptCell?.ToString() ?? "General";

                                if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(email))
                                    continue;

                                // Parse values
                                decimal.TryParse(salaryStr, out decimal salary);
                                DateTime.TryParse(hireDateStr, out DateTime hireDate);
                                
                                // Ensure hireDate is UTC
                                if (hireDate != DateTime.MinValue && hireDate.Kind == DateTimeKind.Unspecified)
                                {
                                    hireDate = new DateTime(hireDate.Year, hireDate.Month, hireDate.Day, hireDate.Hour, hireDate.Minute, hireDate.Second, DateTimeKind.Utc);
                                }

                                // Check if already exists
                                var existing = _db.Employees.FirstOrDefault(e => e.Email == email || e.Document == document);

                                if (existing != null)
                                {
                                    // Update
                                    existing.FirstName = firstName;
                                    existing.LastName = lastName;
                                    existing.Document = document;
                                    existing.Phone = phone;
                                    existing.Email = email;
                                    existing.Position = position;
                                    existing.Salary = salary;
                                    existing.HireDate = hireDate;
                                    existing.Status = status;
                                    existing.EducationLevel = educationLevel;
                                    existing.ProfessionalProfile = professionalProfile;
                                    existing.Address = address;
                                    existing.UpdatedAt = DateTime.UtcNow;
                                    _db.Employees.Update(existing);
                                }
                                else
                                {
                                    // Create new
                                    var newEmployee = new Employee
                                    {
                                        FirstName = firstName,
                                        LastName = lastName,
                                        Document = document,
                                        Phone = phone,
                                        Email = email,
                                        Position = position,
                                        Salary = salary,
                                        HireDate = hireDate,
                                        Status = status,
                                        EducationLevel = educationLevel,
                                        ProfessionalProfile = professionalProfile,
                                        Address = address,
                                        CreatedAt = DateTime.UtcNow,
                                        UpdatedAt = DateTime.UtcNow
                                    };
                                    _db.Employees.Add(newEmployee);
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error in row {row}: {ex.Message}");
                            }
                        }

                        await _db.SaveChangesAsync();
                        ViewBag.Message = "Import completed successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error processing file: {ex.Message}");
            }

            return View();
        }

        /// <summary>
        /// Download Curriculum Vitae in PDF
        /// </summary>
        public async Task<IActionResult> DownloadPDF(int id)
        {
            var employee = await _repository.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            var pdfBytes = _pdfService.GenerarHojaVidaPDF(employee);
            return File(pdfBytes, "application/pdf", $"CV_{employee.FirstName}_{employee.LastName}.pdf");
        }

        // Debug: Import from fixed file path
        [HttpGet]
        [Route("/debug-import")]
        public IActionResult DebugImport()
        {
            try
            {
                string filePath = "/home/jeroc/Documentos/prueba desempeño/TalentoPlus.Web/Empleados.xlsx";
                
                if (!System.IO.File.Exists(filePath))
                    return BadRequest("File not found");

                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension?.Rows ?? 0;
                        int imported = 0;
                        int errors = 0;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                var documentCell = worksheet.Cells[row, 1].Value;
                                var firstNameCell = worksheet.Cells[row, 2].Value;
                                var lastNameCell = worksheet.Cells[row, 3].Value;
                                var birthDateCell = worksheet.Cells[row, 4].Value;
                                var addressCell = worksheet.Cells[row, 5].Value;
                                var phoneCell = worksheet.Cells[row, 6].Value;
                                var emailCell = worksheet.Cells[row, 7].Value;
                                var positionCell = worksheet.Cells[row, 8].Value;
                                var salaryCell = worksheet.Cells[row, 9].Value;
                                var hireDateCell = worksheet.Cells[row, 10].Value;
                                var statusCell = worksheet.Cells[row, 11].Value;
                                var educCell = worksheet.Cells[row, 12].Value;
                                var profileCell = worksheet.Cells[row, 13].Value;
                                var deptCell = worksheet.Cells[row, 14].Value;

                                var firstName = firstNameCell?.ToString() ?? "";
                                var email = emailCell?.ToString() ?? "";

                                if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(email))
                                    continue;

                                imported++;
                            }
                            catch (Exception ex)
                            {
                                errors++;
                                System.Diagnostics.Debug.WriteLine($"Row {row} error: {ex.Message}");
                            }
                        }

                        return Ok(new { total_rows = rowCount, imported, errors, message = $"Debug: {imported} would be imported, {errors} errors" });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
