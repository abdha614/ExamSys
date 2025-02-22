using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogicLayer.Dtos.CourseDto;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;

namespace BusinessLogicLayer.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;

        public CourseService(ICourseRepository courseRepository, ICategoryService categoryService, IMapper mapper)
        {
            _courseRepository = courseRepository;
            _categoryService = categoryService;
            _mapper = mapper;
        }
        public async Task<IEnumerable<CourseGetDto>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CourseGetDto>>(courses);
        }

        public async Task<IEnumerable<CourseGetDto>> GetAllCoursesByCategoryAndProfessorAsync(string categoryName, int professorId)
        {
            var courses = await _courseRepository.GetCoursesByCategoryAndProfessorAsync(categoryName, professorId);
            return _mapper.Map<IEnumerable<CourseGetDto>>(courses);
        }

        public async Task<CourseGetDto> GetCourseByNameAsync(string courseName)
        {
            var course = await _courseRepository.GetByNameAsync(courseName);
            return _mapper.Map<CourseGetDto>(course);
        }
        public async Task<CourseGetDto> AddCourseAsync(string name, int categoryId, int professorId)
        {
            var course = new Course { Name = name, CategoryId = categoryId, ProfessorId = professorId };
            await _courseRepository.AddAsync(course);
            return _mapper.Map<CourseGetDto>(course);
        }
        public async Task<IEnumerable<CourseGetDto>> GetCoursesByProfessorAsync(int professorId)
        {
            var courses = await _courseRepository.GetByProfessorIdAsync(professorId);
            return _mapper.Map<IEnumerable<CourseGetDto>>(courses);
        }
        public async Task<CourseGetDto> GetCourseByNameAndProfessorAsync(string name, int professorId) 
        {
            var course = await _courseRepository.GetCourseByNameAndProfessorAsync(name, professorId);
            return _mapper.Map<CourseGetDto>(course);
        }
        public async Task<CourseGetDto> GetCourseByNameCategoryAndProfessorAsync(string name, int categoryId, int professorId)
        {
            var course = await _courseRepository.GetCourseByNameCategoryAndProfessorAsync(name, categoryId, professorId);
            return _mapper.Map<CourseGetDto>(course);
        }
    }
}
