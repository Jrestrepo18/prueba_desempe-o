using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TalentoPlus.Web.Data;
using TalentoPlus.Web.Models;
using TalentoPlus.Web.Models.Dtos;
using TalentoPlus.Web.Services;

namespace TalentoPlus.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public AuthController(ApplicationDbContext db, IConfiguration configuration, EmailService emailService)
        {
            _db = db;
            _configuration = configuration;
            _emailService = emailService;
        }

        /// <summary>
        /// Public employee registration (self-registration)
        /// </summary>
    [HttpPost("register")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Register([FromBody] EmployeeCreateDto dto)
        {
            try
            {
                // Validations
                if (string.IsNullOrWhiteSpace(dto.Document))
                    return BadRequest(new { message = "Document is required" });
                
                if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                    return BadRequest(new { message = "First name and last name are required" });
                
                if (string.IsNullOrWhiteSpace(dto.Email))
                    return BadRequest(new { message = "Email is required" });

                // Validate email format
                if (!IsValidEmail(dto.Email))
                    return BadRequest(new { message = "Invalid email format" });

                // Validate that the employee does not exist
                var existingByDocument = _db.Employees.FirstOrDefault(e => e.Document == dto.Document);
                if (existingByDocument != null)
                    return BadRequest(new { message = "An employee with this document already exists" });

                var existingByEmail = _db.Employees.FirstOrDefault(e => e.Email == dto.Email);
                if (existingByEmail != null)
                    return BadRequest(new { message = "An employee with this email already exists" });

                // Create employee
                var hireDate = dto.HireDate ?? DateTime.UtcNow;
                var employee = new Employee
                {
                    FirstName = dto.FirstName.Trim(),
                    LastName = dto.LastName.Trim(),
                    Document = dto.Document.Trim(),
                    Email = dto.Email.Trim().ToLower(),
                    Phone = dto.Phone?.Trim(),
                    DepartmentId = dto.DepartmentId > 0 ? dto.DepartmentId : null,
                    Status = "Active",
                    Salary = dto.Salary ?? 0, // Default to 0 if not provided
                    HireDate = hireDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _db.Employees.Add(employee);
                await _db.SaveChangesAsync();

                // Send welcome email
                var emailBody = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
                            .content {{ padding: 20px; background-color: #f9f9f9; border: 1px solid #ddd; border-radius: 0 0 5px 5px; }}
                            .footer {{ margin-top: 20px; text-align: center; font-size: 12px; color: #666; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h2>¡Bienvenido a TalentoPlus!</h2>
                            </div>
                            <div class='content'>
                                <p>Estimado/a <strong>{employee.FirstName} {employee.LastName}</strong>,</p>
                                <p>Tu registro en la plataforma TalentoPlus ha sido completado exitosamente.</p>
                                <p>Ya puedes acceder a la plataforma cuando el administrador te habilite.</p>
                                <h3>Datos de tu registro:</h3>
                                <ul>
                                    <li><strong>Documento:</strong> {employee.Document}</li>
                                    <li><strong>Email:</strong> {employee.Email}</li>
                                    <li><strong>Nombre:</strong> {employee.FirstName} {employee.LastName}</li>
                                </ul>
                                <p>Para autenticarte, utiliza tu documento y correo electrónico.</p>
                                <p>Si tienes preguntas, no dudes en contactar al administrador.</p>
                            </div>
                            <div class='footer'>
                                <p>© 2025 TalentoPlus S.A.S - Todos los derechos reservados</p>
                            </div>
                        </div>
                    </body>
                    </html>
                ";

                try
                {
                    await _emailService.SendEmailAsync(employee.Email, "¡Bienvenido a TalentoPlus!", emailBody);
                }
                catch (Exception emailEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Warning: Email failed but registration completed: {emailEx.Message}");
                    // Registration is successful even if email fails
                }

                return Ok(new LoginResponseDto { Message = "Registro exitoso. Se ha enviado un correo de confirmación." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error en el registro: {ex.Message}" });
            }
        }

        /// <summary>
        /// Employee login (returns JWT)
        /// </summary>
    [HttpPost("login")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public ActionResult<LoginResponseDto> Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                // Validations
                if (string.IsNullOrWhiteSpace(dto.Document))
                    return BadRequest(new { message = "Document is required" });

                if (string.IsNullOrWhiteSpace(dto.Email))
                    return BadRequest(new { message = "Email is required" });

                // Search employee by document and email
                var employee = _db.Employees.FirstOrDefault(e => 
                    e.Document.ToLower() == dto.Document.ToLower().Trim() && 
                    e.Email.ToLower() == dto.Email.ToLower().Trim());

                if (employee == null)
                {
                    return Unauthorized(new { message = "Invalid credentials. Please verify your document and email." });
                }

                if (employee.Status != "Active")
                {
                    return Unauthorized(new { message = "Your account is not active. Please contact the administrator." });
                }

                // Generate JWT
                var token = GenerateJwtToken(employee);

                return Ok(new LoginResponseDto 
                { 
                    Token = token,
                    Message = "Authentication successful"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error in login: {ex.Message}" });
            }
        }

        private string GenerateJwtToken(Employee employee)
        {
            var jwtSecret = _configuration["Jwt:Secret"] ?? "TalentoPlus_Secret_Key_2025_VeryLongAndSecure123456789";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "TalentoPlus";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "TalentoPlus";
            var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "1440");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new Claim(ClaimTypes.Email, employee.Email),
                new Claim("Document", employee.Document),
                new Claim(ClaimTypes.Name, $"{employee.FirstName} {employee.LastName}")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                Issuer = jwtIssuer,
                Audience = jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
