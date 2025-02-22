using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogicLayer.Dtos.CategoryDto;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;

namespace BusinessLogicLayer.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryGetDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryGetDto>>(categories);
        }
        public async Task<CategoryGetDto> GetCategoryByIdAsync(int categoryId)
        { 
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            return _mapper.Map<CategoryGetDto>(category); 
        }
        public async Task<CategoryGetDto> GetCategoryByNameAsync(string categoryName)
        {
            var category = await _categoryRepository.GetByNameAsync(categoryName);
            return _mapper.Map<CategoryGetDto>(category);
        }
        public async Task<CategoryGetDto> AddCategoryAsync(string name, int professorId)
        {
            var category = new Category { Name = name, ProfessorId = professorId };
            await _categoryRepository.AddAsync(category);
            return _mapper.Map<CategoryGetDto>(category);
        }

        public async Task<IEnumerable<CategoryGetDto>> GetCategoriesByProfessorAsync(int professorId)
        {
            var categories = await _categoryRepository.GetByProfessorIdAsync(professorId);
            return _mapper.Map<IEnumerable<CategoryGetDto>>(categories);
        }
        public async Task<CategoryGetDto> GetCategoryByNameAndProfessorAsync(string name, int professorId)
        {
            var category = await _categoryRepository.GetCategoryByNameAndProfessorAsync(name, professorId);
            return _mapper.Map<CategoryGetDto>(category);
        }
    }
}
