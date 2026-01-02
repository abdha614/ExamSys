using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogicLayer.Dtos.CourseDto;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using BusinessLogicLayer.Dtos.CategoryDto;
using BusinessLogicLayer.Dtos.LectureDto;

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
        //public async Task<ProfessorDataDto> GetCoursesAndCategoriesByProfessorAsync(int professorId)
        //{
        //    var courses = await _courseRepository.GetCoursesWithCategoriesByProfessorAsync(professorId);

        //    // Map to DTOs
        //    var courseDtos = courses.Select(c => new CourseGetDto
        //    {
        //        Id = c.Id,
        //        Name = c.Name,
        //        Category = new CategoryGetDto
        //        {
        //            Id = c.Category.Id,
        //            Name = c.Category.Name
        //        }
        //    }).ToList();

        //    return new ProfessorDataDto
        //    {
        //        Courses = courseDtos,
        //        Categories = courseDtos.Select(c => c.Category)
        //                    .GroupBy(category => category.Id)
        //                    .Select(group => group.First())
        //                    .ToList()
        //    };

        //}
        public async Task<bool> DeleteCourseWithDependenciesAsync(int courseId)
        {
           var res=  await _courseRepository.DeleteCourseWithDependenciesAsync(courseId);
            return res;
        }
        public async Task AddCourseAsync(CourseAddDto courseAddDto)
        {
            // Use AutoMapper to map CategoryAddDto to Category
            var course = _mapper.Map<Course>(courseAddDto);

            // Add the category to the repository
            await _courseRepository.AddAsync(course);
        }
        public async Task<List<LectureGetDto>> GetLecturesByCourseAsync(int courseId)
        {
            var lectures = await _courseRepository.GetLecturesByCourseAsync(courseId);

            return lectures.Select(lecture => new LectureGetDto
            {
                Id = lecture.Id,
                LectureName = lecture.LectureName
            }).ToList();
        }
        public async Task<int?> GetCourseIdByNameAndProfessorAsync(string name, int professorId)
        {
            return await _courseRepository.GetCourseIdByNameAndProfessorAsync(name, professorId);
        }

        public async Task<IEnumerable<CourseGetDto>> GetCoursesWithAvailableLecturesByProfessorAsync(int professorId)
        {
            var courses = await _courseRepository.GetCoursesWithAvailableLecturesAsync(professorId);

            var courseDtos = courses.Select(c => new CourseGetDto
            {
                Id = c.Id,
                Name = c.Name,
                Lectures = c.Lectures.Select(l => new LectureGetDto
                {
                    Id = l.Id,
                    LectureName = l.LectureName,
                    FileNames = l.Files.Select(f => f.FileName).ToList() // include existing file names
                }).ToList()
            });

            return courseDtos;
        }

        public async Task<IEnumerable<CourseGetDto>> GetCoursesWithLecturesAndFilesByProfessorAsync(int professorId)
        {
            var courses = await _courseRepository.GetCoursesWithLecturesAndFilesAsync(professorId);

            // Map to DTOs
            var courseDtos = courses.Select(c => new CourseGetDto
            {
                Id = c.Id,
                Name = c.Name,
                Lectures = c.Lectures.Select(l => new LectureGetDto
                {
                    Id = l.Id,
                    LectureName = l.LectureName,
                    Files = l.Files.Select(f => new LectureFileGetDto
                    {
                        Id = f.Id,
                        FileName = f.FileName,
                        FilePath = f.FilePath
                    }).ToList()
                }).ToList()
            });

            return courseDtos;
        }

    }
}