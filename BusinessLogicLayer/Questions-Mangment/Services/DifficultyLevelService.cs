using AutoMapper;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class DifficultyLevelService : IDifficultyLevelService
    {
        private readonly IDifficultyLevelRepository _difficultyLevelRepository;
        private readonly IMapper _mapper;  // Add AutoMapper


        public DifficultyLevelService(IDifficultyLevelRepository difficultyLevelRepository, IMapper mapper)
        {
            _difficultyLevelRepository = difficultyLevelRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DifficultyLevelDto>> GetAllDifficultyLevelsAsync()
        {
            var difficultyLevels = await _difficultyLevelRepository.GetAllDifficultyLevelsAsync();
            var DifficultyLevelDtos = _mapper.Map<IEnumerable<DifficultyLevelDto>>(difficultyLevels);
            return DifficultyLevelDtos;
        }
        public async Task<int> GetDifficultyLevelByNameAsync(string difficultyLevelName)
        {
            if (string.IsNullOrWhiteSpace(difficultyLevelName))
            {
                return 1; 
            }
            var difficultyLevel = await _difficultyLevelRepository.GetByNameAsync(difficultyLevelName);
            if (difficultyLevel == null)
            {
                throw new ArgumentException($"Difficulty level '{difficultyLevelName}' not found.");
            }
            return difficultyLevel.Id;
        }
        public async Task<List<DifficultyLevelDto>> GetDifficultyLevelsByProfessorIdAsync(int professorId)
        {
            var difficultyLevels = await _difficultyLevelRepository.GetAllAsync(); // Await data before filtering

            var filteredDifficultyLevels = difficultyLevels
                .Select(dl => new DifficultyLevelDto
                {
                    Id = dl.Id,
                    Name = dl.Level
                })
                .ToList();

            return filteredDifficultyLevels;
        }

    }
}
