using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class DifficultyLevelRepository : GenericRepository<DifficultyLevel>, IDifficultyLevelRepository
    {
        public DifficultyLevelRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<DifficultyLevel>> GetAllDifficultyLevelsAsync()
        {
            return await _dbSet.ToListAsync(); // Assuming DbSet<DifficultyLevel> exists in ApplicationDbContext
        }
        public async Task<DifficultyLevel> GetByNameAsync(string name)
        {
            string normalizedInput = name.Replace(" ", "").Trim().ToLower();
             return await _dbSet
                .FirstOrDefaultAsync(dl =>
                    dl.Level.Replace(" ", "").Trim().ToLower() == normalizedInput);
             
        }
       



    }
}
