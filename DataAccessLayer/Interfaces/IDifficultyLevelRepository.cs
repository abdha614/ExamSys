using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IDifficultyLevelRepository : IGenericRepository<DifficultyLevel>
    {
        Task<IEnumerable<DifficultyLevel>> GetAllDifficultyLevelsAsync();
        Task<DifficultyLevel> GetByNameAsync(string name);
    }
}
