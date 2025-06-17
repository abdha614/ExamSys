using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogicLayer.Dtos.CategoryDto;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using DataAccessLayer.Dtos;
using BusinessLogicLayer.Dtos.LectureDto;

namespace BusinessLogicLayer.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private IDifficultyLevelService _difficultyLevelService;

        

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, IDifficultyLevelService difficultyLevelService
 )
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _difficultyLevelService = difficultyLevelService;
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
        public async Task<IEnumerable<CategoryGetDto>> GetCategoriesByProfessorAsync(int professorId)
        {
            var categories = await _categoryRepository.GetByProfessorIdAsync(professorId);
            return _mapper.Map<IEnumerable<CategoryGetDto>>(categories);
        }
        public async Task<int?> GetCategoryIdByNameAndProfessorAsync(string name, int professorId)
        {
            var categoryId = await _categoryRepository.GetCategoryIdByNameAndProfessorAsync(name, professorId);
            return categoryId;
        }

        public async Task<IEnumerable<CategoryGetDto>> GetAllCategoryWithProfessorEmailAsyncs(int? professorId = null)
        {
            var categories = await _categoryRepository.GetAllCategoryWithProfessorEmailAsyncs(professorId);
            return _mapper.Map<IEnumerable<CategoryGetDto>>(categories);
        }
        //public async Task<IEnumerable<CategoryWithProfessorEmailDto>> GetAllCategoryWithProfessorEmailAsyncs()
        //{
        //    // Call the repository method
        //    var categories = await _categoryRepository.GetAllCategoryWithProfessorEmailAsyncs();

        //    // Return the result (no further mapping required because the repository already returns DTOs)
        //    return categories;
        //}
        public async Task RemoveCategoryFromProfessorAsync(int professorId, int categoryId)
        {
            await _categoryRepository.RemoveCategoryFromProfessorAsync(professorId, categoryId);
        }
        //public async Task<ProfessorDataDto> GetAllRelatedDataForProfessorAsync(int professorId)
        //{
        //    var categories = await _categoryRepository.GetCategoriesWithCoursesAndLecturesAsync(professorId);
        //    var DifficultyLevel = await _difficultyLevelService.GetAllDifficultyLevelsAsync();
        //    // Map to DTOs
        //    var categoryDtos = categories.Select(c => new CategoryGetDto
        //    {
        //        Id = c.Id,
        //        Name = c.Name,
        //        Courses = c.Courses.Select(course => new CourseGetDto
        //        {
        //            Id = course.Id,
        //            Name = course.Name,
        //            Lectures = course.Lectures.Select(lecture => new LectureGetDto
        //            {
        //                Id = lecture.Id,
        //                LectureName = lecture.LectureName
        //            }).ToList()
        //        }).ToList()
        //    }).ToList();

        //    // Map question types to DTOs
        //    var DifficultyLevelDtos = DifficultyLevel.Select(qt => new DifficultyLevelDto
        //    {
        //        Id = qt.Id,
        //        Name = qt.Name
        //    }).ToList();

        //    var res =  new ProfessorDataDto
        //    {
        //        Categories = categoryDtos,
        //        DifficultyLevels = DifficultyLevelDtos


        //    };
        //    return res;
        //}
        public async Task<ProfessorDataDto> GetAllRelatedDataForProfessorAsync(int professorId)
        {
            var categories = await _categoryRepository.GetCategoriesWithCoursesAndLecturesAsync(professorId);
            var DifficultyLevel = await _difficultyLevelService.GetAllDifficultyLevelsAsync();

            // Map to DTOs
            var categoryDtos = categories.Select(c => new CategoryGetDto
            {
                Id = c.Id,
                Name = c.Name,
                Courses = c.Courses.Select(course => new CourseGetDto
                {
                    Id = course.Id,
                    Name = course.Name,
                    Lectures = course.Lectures.Select(lecture => new LectureGetDto
                    {
                        Id = lecture.Id,
                        LectureName = lecture.LectureName
                    }).ToList()
                }).ToList()
            }).ToList();

            // Extract courses into a separate list
            var courses = categories.SelectMany(c => c.Courses)
                .Select(course => new CourseGetDto
                {
                    Id = course.Id,
                    Name = course.Name,
                    Lectures = course.Lectures.Select(lecture => new LectureGetDto
                    {
                        Id = lecture.Id,
                        LectureName = lecture.LectureName
                    }).ToList()
                }).ToList();

            // Map difficulty levels to DTOs
            var DifficultyLevelDtos = DifficultyLevel.Select(qt => new DifficultyLevelDto
            {
                Id = qt.Id,
                Name = qt.Name
            }).ToList();

            var res = new ProfessorDataDto
            {
                Categories = categoryDtos,
                DifficultyLevels = DifficultyLevelDtos,
                Courses = courses // Include courses
            };

            return res;
        }

        public async Task<IEnumerable<CategoryWithCoursesDto>> GetCategoriesWithCoursesByProfessorIdAsync(int professorId)
        {
            // Step 1: Fetch categories including their courses for this professor
            var categories = await _categoryRepository.GetCategoriesWithCoursesByProfessorIdAsync(professorId);

            // Step 2: Map to DTOs
            var result = categories.Select(category => new CategoryWithCoursesDto
            {
                Id = category.Id,
                Name = category.Name,
                Courses = category.Courses
                    .Select(course => new CourseGetDto
                    {
                        Id = course.Id,
                        Name = course.Name,
                        CategoryId = course.CategoryId
                    }).ToList()
            });

            return result;
        }
        public async Task AddCategoryAsync(CategoryAddDto categoryAddDto)
        {
            // Use AutoMapper to map CategoryAddDto to Category
            var category = _mapper.Map<Category>(categoryAddDto);

            // Add the category to the repository
            await _categoryRepository.AddAsync(category);
        }
        public async Task<List<CourseGetDto>> GetCoursesWithQuestionsByCategoryAsync(int categoryId)
        {
            var courses = await _categoryRepository.GetCoursesByCategoryAsync(categoryId);

            return courses.Select(course => new CourseGetDto
            {
                Id = course.Id,
                Name = course.Name
            }).ToList();
        }
        public async Task<List<CategoryGetDto>> GetCategoriesByProfessorIdAsync(int professorId)
        {
            var categories = await _categoryRepository.GetAllAsync(); // Await the data before using Where()

            var filteredCategories = categories
                .Where(c => c.ProfessorId == professorId) // Now `Where()` works
                .Select(c => new CategoryGetDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();

            return filteredCategories;
        }

        public async Task<CategoryGetDto> GetCategoryByNameAndProfessorAsync(string name, int professorId)
        {
            var category = await _categoryRepository.GetCategoryByNameAndProfessorAsync(name, professorId);
            return _mapper.Map<CategoryGetDto>(category);
        }

    }
}
