using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataAccessLayer.Data;
using DataAccessLayer.Dtos;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly IMapper _mapper; // Inject the mapper instance
        public CategoryRepository(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }
        //public async Task<IEnumerable<Category>> GetAllCategoryWithProfessorEmailAsync()
        //{
        //    var result = await _context.Categories
        //                         .Include(c => c.Professor) // Eagerly load Professor data
        //                         .ToListAsync();
        //    return result;
        //}

        public async Task<Category> GetByNameAsync(string categoryName)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == categoryName);
        }
        public async Task<IEnumerable<Category>> GetByProfessorIdAsync(int professorId)
        {
            return await _dbSet.Where(c => c.ProfessorId == professorId).ToListAsync();
        }
        public async Task<int?> GetCategoryIdByNameAndProfessorAsync(string name, int professorId)
        {
            return await _dbSet
                .Where(c => c.Name == name && c.ProfessorId == professorId)
                .Select(c => (int?)c.Id) // Ensures null handling
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CategoryWithProfessorEmailDto>> GetAllCategoryWithProfessorEmailAsyncs(int? professorId = null)
        {
            var query = _dbSet.AsQueryable();

            if (professorId.HasValue)
            {
                query = query.Where(c => c.ProfessorId == professorId.Value);
            }

            var result = await query
                .ProjectTo<CategoryWithProfessorEmailDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return result;
        }
        public async Task<bool> RemoveCategoryFromProfessorAsync(int professorId, int categoryId)
        {
            // Find the category that belongs to the professor
            var category = await _dbSet.FirstOrDefaultAsync(c => c.Id == categoryId && c.ProfessorId == professorId);

            if (category != null)
            {
                // 1. First handle all questions and their dependencies
                var questions = await _context.Questions
                    .Where(q => q.CategoryId == categoryId)
                    .ToListAsync();

                foreach (var question in questions)
                {
                    // Delete all answers for each question
                    var answers = await _context.Answers
                        .Where(a => a.QuestionId == question.Id)
                        .ToListAsync();
                    _context.Answers.RemoveRange(answers);
                }
                _context.Questions.RemoveRange(questions);

                // 2. Handle courses and their dependencies
                var courses = await _context.Courses
                    .Where(c => c.CategoryId == categoryId)
                    .ToListAsync();

                foreach (var course in courses)
                {
                    // Delete lectures for each course
                    var lectures = await _context.lectures
                        .Where(l => l.CourseId == course.Id)
                        .ToListAsync();

                    foreach (var lecture in lectures)
                    {
                        // Delete questions for each lecture (if any)
                        var lectureQuestions = await _context.Questions
                            .Where(q => q.LectureId == lecture.Id)
                            .ToListAsync();

                        foreach (var question in lectureQuestions)
                        {
                            var answers = await _context.Answers
                                .Where(a => a.QuestionId == question.Id)
                                .ToListAsync();
                            _context.Answers.RemoveRange(answers);
                        }
                        _context.Questions.RemoveRange(lectureQuestions);
                    }
                    _context.lectures.RemoveRange(lectures);
                    _context.Courses.Remove(course);
                }

                // 3. Finally remove the category itself
                _dbSet.Remove(category);

                await _context.SaveChangesAsync();
                return true;
            }

            return false; // Return false if no category was found
        }
        public async Task<IEnumerable<Category>> GetCategoriesWithCoursesAndLecturesAsync(int professorId)
        {
            var res = await _dbSet
                  .Include(c => c.Courses) // Load courses linked to categories
                  .ThenInclude(course => course.Lectures) // Load lectures linked to courses
                  .Where(c => c.ProfessorId == professorId) // Filter by professor
                  .ToListAsync();
            return res;

             
        }
        public async Task<IEnumerable<Category>> GetCategoriesWithCoursesByProfessorIdAsync(int professorId)
        {
            var res= await _dbSet
                .Where(c => c.ProfessorId == professorId)
                .Include(c => c.Courses)
                .ToListAsync();
            return res;
        }
        public async Task<List<Course>> GetCoursesByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Where(c => c.Id == categoryId) // Filter by category ID
                .SelectMany(c => c.Courses)     // Flatten courses for the category
                .Where(course => course.Questions.Any()) // Include courses that have questions
                .ToListAsync();
        }
        public async Task<Category> GetCategoryByNameAndProfessorAsync(string name, int professorId)
        { 
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == name && c.ProfessorId == professorId);
        }
    }
}
