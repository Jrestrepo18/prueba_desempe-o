using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Web.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The department name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Description { get; set; }

        // Inverse relationship with Employee
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
