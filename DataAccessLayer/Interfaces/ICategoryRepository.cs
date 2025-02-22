using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category> GetByNameAsync(string categoryName);
        Task<IEnumerable<Category>> GetByProfessorIdAsync(int professorId);
        Task<Category> GetCategoryByNameAndProfessorAsync(string name, int professorId);
    }
}