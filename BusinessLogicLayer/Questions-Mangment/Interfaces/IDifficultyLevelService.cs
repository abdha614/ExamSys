using BusinessLogicLayer.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IDifficultyLevelService
    {
        Task<IEnumerable<DifficultyLevelDto>> GetAllDifficultyLevelsAsync();
        Task<int> GetDifficultyLevelByNameAsync(string difficultyLevelName);
        Task<List<DifficultyLevelDto>> GetDifficultyLevelsByProfessorIdAsync(int professorId);
    }
}
