using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Course>> GetCoursesByCategoryAndProfessorAsync(string categoryName, int professorId)
        {
            return await _dbSet
                .Include(c => c.Category)  // Load Category
                .Where(c => c.Category.Name == categoryName && c.ProfessorId == professorId)  // Filter by Category and professorId
                .ToListAsync();
        }

        public async Task<Course> GetByNameAsync(string courseName)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == courseName);
        }
        public async Task<IEnumerable<Course>> GetByProfessorIdAsync(int professorId)
        {
            return await _context.Courses.Where(c => c.ProfessorId == professorId).ToListAsync();
        }
        public async Task<Course> GetCourseByNameAndProfessorAsync(string name, int professorId)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.Name == name && c.ProfessorId == professorId); 
        }
        public async Task<Course> GetCourseByNameCategoryAndProfessorAsync(string name, int categoryId, int professorId)
        {
            return await _context.Courses
                                 .FirstOrDefaultAsync(c => c.Name == name && c.CategoryId == categoryId && c.ProfessorId == professorId);
        }
        public async Task<IEnumerable<Course>> GetCoursesWithCategoriesByProfessorAsync(int professorId)
        {
            var res =  await _dbSet
                .Include(c => c.Category) // Include Category navigation property
                .Where(c => c.ProfessorId == professorId)
                .ToListAsync();
            return res;
        }
        public async Task<bool> DeleteCourseWithDependenciesAsync(int courseId)
        {
            // Find the course by ID
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);

            if (course != null)
            {
                // Step 1: Find all lectures associated with this course
                var lectures = await _context.lectures
                    .Where(l => l.CourseId == courseId)
                    .ToListAsync();

                // Step 2: Find all questions directly associated with this course
                var directQuestions = await _context.Questions
                    .Where(q => q.CourseId == courseId)
                    .ToListAsync();

                // Step 3: For each lecture, delete its related questions and their answers
                foreach (var lecture in lectures)
                {
                    var questions = await _context.Questions
                        .Where(q => q.LectureId == lecture.Id)
                        .ToListAsync();

                    foreach (var question in questions)
                    {
                        // Find and delete all answers related to the question
                        var answers = await _context.Answers
                            .Where(a => a.QuestionId == question.Id)
                            .ToListAsync();

                        _context.Answers.RemoveRange(answers);
                    }

                    _context.Questions.RemoveRange(questions);
                }

                // Step 4: Delete answers for direct questions
                foreach (var question in directQuestions)
                {
                    var answers = await _context.Answers
                        .Where(a => a.QuestionId == question.Id)
                        .ToListAsync();

                    _context.Answers.RemoveRange(answers);
                }

                // Step 5: Remove direct questions
                _context.Questions.RemoveRange(directQuestions);

                // Step 6: Remove all lectures under the course
                _context.lectures.RemoveRange(lectures);

                // Step 7: Remove the course itself
                _context.Courses.Remove(course);

                // Save changes to the database
                await _context.SaveChangesAsync();

                return true;
            }

            return false; // Course not found
        }
        public async Task<List<Lecture>> GetLecturesByCourseAsync(int courseId)
        {
            return await _dbSet
                .Where(c => c.Id == courseId) // Filter by course ID
                .SelectMany(c => c.Lectures)  // Flatten lectures for the course
                .Where(lecture => lecture.Questions.Any()) // Include lectures with questions
                .ToListAsync();
        }
        public async Task<int?> GetCourseIdByNameAndProfessorAsync(string name, int professorId)
        {
            return await _context.Courses
                .Where(c => c.Name == name && c.ProfessorId == professorId)
                .Select(c => (int?)c.Id) // Ensures null handling
                .FirstOrDefaultAsync();
        }
    }
}
