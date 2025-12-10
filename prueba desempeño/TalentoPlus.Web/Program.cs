using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TalentoPlus.Web.Data;
using TalentoPlus.Web.Models;
using TalentoPlus.Web.Repositories;
using TalentoPlus.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== Configuración de Base de Datos (PostgreSQL) =====
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL") 
    ?? "Host=localhost;Port=5432;Database=prueba_jero;Username=envyguard_user;Password=jE15QhCwINzUNUw1FdclOB8YqZOE89";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// ===== ASP.NET Core Identity =====
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ===== JWT Authentication =====
var jwtSecret = builder.Configuration["Jwt:Secret"] 
    ?? "TalentoPlus_Secret_Key_2025_VeryLongAndSecure123456789";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "TalentoPlus";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "TalentoPlus";

// AddIdentity ya configura la autenticación con cookies
// Solo agregamos JWT Bearer como esquema adicional
builder.Services.AddAuthentication()
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ===== Controllers, Views and Services =====
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IRepositoryEmployee, RepositoryEmployee>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<PdfService>();
builder.Services.AddScoped<JwtService>();

var app = builder.Build();

// ===== Apply Database Migrations and Seed Data Automatically =====
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
        
        // Initialize seed data (roles, departments, admin user)
        await SeedData.InitializeAsync(scope.ServiceProvider);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️  Migration or seed failed: {ex.Message}");
    Console.WriteLine($"   This is expected if PostgreSQL credentials are not configured correctly.");
    Console.WriteLine($"   Please update the connection string in appsettings.json with the correct credentials.");
}

// ===== Middleware =====
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers(); // Para API routes

// Escuchar en puerto 5003 en localhost
var port = Environment.GetEnvironmentVariable("PORT") ?? "5003";
app.Run($"http://localhost:{port}");
