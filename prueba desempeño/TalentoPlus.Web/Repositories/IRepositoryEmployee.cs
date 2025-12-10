using System.Collections.Generic;
using System.Threading.Tasks;
using TalentoPlus.Web.Models;

namespace TalentoPlus.Web.Repositories
{
    public interface IRepositoryEmployee
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(int id);
    }
}
