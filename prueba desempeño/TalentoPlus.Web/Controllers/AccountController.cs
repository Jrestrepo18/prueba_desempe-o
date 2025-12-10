using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Web.Data;
using TalentoPlus.Web.Models;
using TalentoPlus.Web.Services;

namespace TalentoPlus.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly JwtService _jwtService;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db, JwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _db = db;
            _jwtService = jwtService;
        }

        /// <summary>
        /// GET: Account/Login
        /// </summary>
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// POST: Account/Login
        /// Soporta login con email/documento como usuario y contraseña real (usando ASP.NET Identity)
        /// También soporta legacy login con documento como contraseña para empleados sin contraseña real
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Por favor ingresa usuario y contraseña");
                return View();
            }

            IdentityUser? identityUser = null;
            Employee? employee = null;

            // Primero, intentar login con ASP.NET Identity (contraseña real)
            // Buscar usuario por email o username
            identityUser = await _userManager.FindByEmailAsync(username);
            if (identityUser == null)
            {
                identityUser = await _userManager.FindByNameAsync(username);
            }

            // Si encontramos usuario en Identity, verificar contraseña
            if (identityUser != null)
            {
                var isPasswordValid = await _userManager.CheckPasswordAsync(identityUser, password);
                if (isPasswordValid)
                {
                    // Buscar empleado asociado
                    employee = _db.Employees.FirstOrDefault(e => e.Email == identityUser.Email && e.Status == "Active");
                    
                    if (employee != null || identityUser.Email == "admin@talentoplus.local")
                    {
                        // Sign in - Esta es la línea crítica que crea la cookie de autenticación
                        await _signInManager.SignInAsync(identityUser, isPersistent: false);

                        // Generar JWT
                        var roles = await _userManager.GetRolesAsync(identityUser);
                        var token = _jwtService.GenerateToken(identityUser.Id, identityUser.Email ?? "", roles.ToList());
                        
                        // Guardar token en cookie
                        Response.Cookies.Append("jwt_token", token, new Microsoft.AspNetCore.Http.CookieOptions
                        {
                            HttpOnly = true,
                            Secure = false, // Cambiar a true en producción
                            SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
                            Expires = DateTimeOffset.UtcNow.AddHours(24)
                        });

                        // Debug: log the roles
                        System.Console.WriteLine($"[LOGIN] User: {identityUser.Email}, Roles: {string.Join(", ", roles)}");

                        // Redirigir al Dashboard si es Admin, si no a Employees
                        if (roles.Contains("Admin"))
                        {
                            System.Console.WriteLine($"[LOGIN] Redirecting Admin to Dashboard and Employees section");
                            // Redirect to Dashboard and indicate we want to go to the Employees section
                            return RedirectToAction("Index", "Dashboard", new { gotoSection = "employees" });
                        }
                        System.Console.WriteLine($"[LOGIN] Redirecting User to Employees");
                        return RedirectToAction("Index", "Employees");
                    }
                }
            }

            // Fallback: Legacy login con documento como contraseña
            employee = _db.Employees.FirstOrDefault(e => 
                (e.Document.ToLower() == username.ToLower() || e.Email.ToLower() == username.ToLower()) &&
                e.Document == password && // La contraseña debe ser igual al documento
                e.Status == "Active");

            if (employee != null)
            {
                // Crear o actualizar usuario en Identity si no existe
                identityUser = await _userManager.FindByEmailAsync(employee.Email);
                
                if (identityUser == null)
                {
                    identityUser = new IdentityUser
                    {
                        UserName = employee.Email,
                        Email = employee.Email,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(identityUser, password);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Error al procesar el login");
                        return View();
                    }

                    // Asignar rol User
                    await _userManager.AddToRoleAsync(identityUser, "User");
                }

                // Sign in
                await _signInManager.SignInAsync(identityUser, isPersistent: false);

                // Generar JWT
                var roles = await _userManager.GetRolesAsync(identityUser);
                var token = _jwtService.GenerateToken(identityUser.Id, employee.Email, roles.ToList());
                
                // Guardar token en cookie
                Response.Cookies.Append("jwt_token", token, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // Cambiar a true en producción
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(24)
                });

                // Redirigir al Dashboard si es Admin, si no a Employees
                if (roles.Contains("Admin"))
                {
                    // When admin logs in via legacy employee flow, also ask dashboard to navigate to employees
                    return RedirectToAction("Index", "Dashboard", new { gotoSection = "employees" });
                }
                return RedirectToAction("Index", "Employees");
            }

            ModelState.AddModelError(string.Empty, "Usuario, contraseña inválidos o cuenta inactiva");
            return View();
        }

        /// <summary>
        /// GET: Account/Register
        /// </summary>
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// POST: Account/Register
        /// Registra un nuevo empleado con documento y contraseña
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string document, string email, string firstName, string lastName, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(document) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Por favor completa todos los campos");
                return View();
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Las contraseñas no coinciden");
                return View();
            }

            // Verificar si empleado ya existe
            if (_db.Employees.Any(e => e.Document == document || e.Email.ToLower() == email.ToLower()))
            {
                ModelState.AddModelError(string.Empty, "El documento o email ya está registrado");
                return View();
            }

            // Crear empleado
            var employee = new Employee
            {
                Document = document.Trim(),
                Email = email.Trim().ToLower(),
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Status = "Active",
                // Salary and HireDate are non-nullable in the DB migration. Provide safe defaults here
                Salary = 0.00m,
                HireDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();

            // Crear usuario en Identity
            var user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Generar JWT
                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtService.GenerateToken(user.Id, email, roles.ToList());
                
                Response.Cookies.Append("jwt_token", token, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(24)
                });

                return RedirectToAction("Index", "Employees");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }

        /// <summary>
        /// <summary>
        /// Logout
        /// </summary>
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Dashboard");
        }

        /// <summary>
        /// Helper method to redirect to local URL
        /// </summary>
        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
