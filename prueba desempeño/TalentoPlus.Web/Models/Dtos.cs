namespace TalentoPlus.Web.Models.Dtos
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Document { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal? Salary { get; set; }
        public DateTime? HireDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string EducationLevel { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
    }

    public class EmployeeCreateDto
    {
        public string Document { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Position { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? HireDate { get; set; }
        public string? Status { get; set; }
        public string? EducationLevel { get; set; }
        public int DepartmentId { get; set; }
    }

    public class LoginRequestDto
    {
        public string Document { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
