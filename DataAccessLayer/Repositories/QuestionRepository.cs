using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataAccessLayer.Data;
using DataAccessLayer.Dtos;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
    {
        private readonly IMapper _mapper; // Inject the mapper instance
        public QuestionRepository(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        //   public async Task<IEnumerable<Question>> GetQuestionsFilteredAsync(int professorId,
        //int? questionTypeId = null,
        //int? difficultyLevelId = null,
        //int? categoryId = null,
        //int? courseId = null)
        //   {
        //       // Start the query filtered by professorId
        //       var query = _dbSet
        //           .Where(q => q.ProfessorId == professorId) // Filter by professor ID
        //           .Include(q => q.QuestionType)         // Eagerly load related QuestionType
        //           .Include(q => q.DifficultyLevel)      // Eagerly load related DifficultyLevel
        //           .Include(q => q.Category)             // Eagerly load related Category
        //           .Include(q => q.Course)               // Eagerly load related Course
        //           .AsQueryable();

        //       // Apply the filters incrementally based on provided parameters

        //       if (questionTypeId.HasValue)
        //       {
        //           // Apply questionTypeId filter
        //           query = query.Where(q => q.QuestionTypeId == questionTypeId.Value);
        //       }

        //       if (difficultyLevelId.HasValue)
        //       {
        //           // Apply difficultyLevelId filter to the already filtered query
        //           query = query.Where(q => q.DifficultyLevelId == difficultyLevelId.Value);
        //       }

        //       if (categoryId.HasValue)
        //       {
        //           // Apply categoryId filter to the already filtered query
        //           query = query.Where(q => q.CategoryId == categoryId.Value);
        //       }

        //       if (courseId.HasValue)
        //       {
        //           // Apply courseId filter to the already filtered query
        //           query = query.Where(q => q.CourseId == courseId.Value);
        //       }

        //       // Execute the query and return the filtered results
        //       return await query.ToListAsync();
        //   }
        public async Task<IEnumerable<QuestionDto>> GetQuestionsFilteredAsync(
    int professorId,
    int? questionTypeId = null,
    int? difficultyLevelId = null,
    int? categoryId = null,
    int? courseId = null,
    int? lectureId = null) // Add LectureId parameter
        {
            // Start the query filtered by professorId
            var query = _dbSet
                .Where(q => q.ProfessorId == professorId) // Filter by professor ID
                .AsQueryable();

            // Apply the filters incrementally based on provided parameters
            if (questionTypeId.HasValue)
                query = query.Where(q => q.QuestionTypeId == questionTypeId.Value);

            if (difficultyLevelId.HasValue)
                query = query.Where(q => q.DifficultyLevelId == difficultyLevelId.Value);

            if (categoryId.HasValue)
                query = query.Where(q => q.CategoryId == categoryId.Value);

            if (courseId.HasValue)
                query = query.Where(q => q.CourseId == courseId.Value);

            if (lectureId.HasValue) // Apply LectureId filter
                query = query.Where(q => q.LectureId == lectureId.Value);

            // Project directly into QuestionDto
            var result = await query
                .ProjectTo<QuestionDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return result;
        }

        public async Task<bool> DoesQuestionExistAsync(string questionText, int professorId)
        {
            return await _dbSet.AnyAsync(q => q.Text == questionText && q.ProfessorId == professorId);
          
        }
        public async Task<IEnumerable<Question>> GetQuestionsByProfessorAsync(int professorId)
        {
            return await _dbSet
                .Where(q => q.ProfessorId == professorId)    // Filter by professor
                .Include(q => q.QuestionType)            // Eagerly load the related QuestionType
                .Include(q => q.DifficultyLevel)         // Eagerly load the related DifficultyLevel
                .Include(q => q.Category)                // Eagerly load the related Category
            //    .Include(q => q.Course)                  // Eagerly load the related Course
                .ToListAsync();                          // Execute the query asynchronously
        }
        public async Task<Question> GetQuestionByyIdAsync(int id)
        {
            var res = await _context.Questions
                //.Include(q => q.QuestionType)
                //.Include(q => q.DifficultyLevel)
                //.Include(q => q.Category)
                //// .Include(q => q.Course)
                .Include(q => q.Answers) // Include answers
                .FirstOrDefaultAsync(q => q.Id == id);
            return res;
        }
        public async Task<QuestionDto> GetQuestionByIdAsync(int id)
        {
            // Start the query
            var query = _dbSet.Where(q => q.Id == id).AsQueryable();

            // Project directly into QuestionDto using AutoMapper
            var result = await query
                .ProjectTo<QuestionDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return result;
        }
        //////////////////////
        public async Task<IEnumerable<QuestionDto>>GetQuestionsFilteredAdvancedAsync(
    int professorId,
    List<int> questionTypeIds,
    List<int> difficultyLevelIds,
    int? categoryId,
    int? courseId,
    List<int> lectureIds)
        {
            // Initialize query filtered by professorId
            var query = _dbSet.Where(q => q.ProfessorId == professorId).AsQueryable();

            // Apply filters for list-based parameters
            if (questionTypeIds != null && questionTypeIds.Any())
                query = query.Where(q => questionTypeIds.Contains(q.QuestionTypeId));

            if (difficultyLevelIds != null && difficultyLevelIds.Any())
                query = query.Where(q => difficultyLevelIds.Contains(q.DifficultyLevelId));

            if (lectureIds != null && lectureIds.Any())
                query = query.Where(q => lectureIds.Contains(q.LectureId));

            // Apply filters for single-value parameters
            if (categoryId.HasValue)
                query = query.Where(q => q.CategoryId == categoryId.Value);

            if (courseId.HasValue)
                query = query.Where(q => q.CourseId == courseId.Value);

            // Project data into DTO format for optimized results
            var questions = await query.ProjectTo<QuestionDto>(_mapper.ConfigurationProvider).ToListAsync();

            return questions;
        }
        public async Task<List<Question>> GetQuestionsByIdsAsync(List<int> questionIds)
        {
           var res =  await _context.Questions
                .Where(q => questionIds.Contains(q.Id))
                .Include(q => q.Answers)
                .ToListAsync(); // Ensure async call
            return res;
        }
        public async Task DeleteQuestionAndAnswersAsync(int questionId)
        {
            // Load the question with its related answers
            var question = await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == questionId);

            if (question != null)
            {
                // Remove all related answers
                _context.Answers.RemoveRange(question.Answers);

                // Then remove the question
                _context.Questions.Remove(question);

                // Save changes
                await _context.SaveChangesAsync();
            }
        }

        // public async Task<List<Question>> GetQuestionsByTypeAndDifficultyAsync(
        //string type, string difficulty, int count,
        //int professorId, int? courseId, int? categoryId, List<int> lectureIds)
        // {
        //     if (count <= 0)
        //         return new List<Question>();

        //     var query = _context.Questions
        //         .Include(q => q.QuestionType)
        //         .Include(q => q.DifficultyLevel)
        //         .Include(q => q.Lecture)
        //         .AsQueryable();

        //     query = query.Where(q =>
        //         q.QuestionType.Type == type &&
        //         q.DifficultyLevel.Level == difficulty &&
        //         q.ProfessorId == professorId &&
        //         (courseId == null || q.CourseId == courseId) &&
        //         (categoryId == null || q.CategoryId == categoryId) &&
        //         (!lectureIds.Any() || lectureIds.Contains(q.LectureId)));

        //     return await query.OrderBy(q => Guid.NewGuid())
        //                       .Take(count)
        //                       .ToListAsync();
        // }

        //    public async Task<List<QuestionDto>> GetQuestionsByTypeAndDifficultyAsync(
        //string type, string difficulty, int count,
        //int professorId, int? courseId, int? categoryId, List<int> lectureIds)
        //    {
        //        if (count <= 0)
        //            return new List<QuestionDto>();

        //        var query = _context.Questions
        //            .Include(q => q.QuestionType)
        //            .Include(q => q.DifficultyLevel)
        //            .Include(q => q.Lecture)
        //            .AsQueryable();

        //        query = query.Where(q =>
        //            q.QuestionType.Type == type &&
        //            q.DifficultyLevel.Level == difficulty &&
        //            q.ProfessorId == professorId &&
        //            (courseId == null || q.CourseId == courseId) &&
        //            (categoryId == null || q.CategoryId == categoryId) &&
        //            (!lectureIds.Any() || lectureIds.Contains(q.LectureId)));

        //        var questions = await query.OrderBy(q => Guid.NewGuid())
        //                                   .Take(count)
        //                                   .ProjectTo<QuestionDto>(_mapper.ConfigurationProvider)
        //                                   .ToListAsync();

        //        return questions;
        //    }
        //  public async Task<List<QuestionDto>> GetQuestionsByTypeAndDifficultyAsync(
        //string type, string difficulty, int count, int professorId, int? courseId,
        //int? categoryId, List<int> lectureIds)
        //  {
        //      if (count <= 0)
        //          return new List<QuestionDto>();

        //      var baseQuery = _context.Questions
        //          .Include(q => q.QuestionType)
        //          .Include(q => q.DifficultyLevel)
        //          .Include(q => q.Lecture)
        //          .Where(q => q.QuestionType.Type == type &&
        //                      q.DifficultyLevel.Level == difficulty &&
        //                      q.ProfessorId == professorId &&
        //                      (courseId == null || q.CourseId == courseId) &&
        //                      (categoryId == null || q.CategoryId == categoryId) &&
        //                      (lectureIds.Count == 0 || lectureIds.Contains(q.LectureId)));

        //      if (!lectureIds.Any())
        //      {
        //          return await baseQuery.OrderBy(q => Guid.NewGuid()).Take(count)
        //              .ProjectTo<QuestionDto>(_mapper.ConfigurationProvider)
        //              .ToListAsync();
        //      }

        //      // Step 1: Shuffle lectures for randomness
        //      var shuffledLectureIds = lectureIds.OrderBy(_ => Guid.NewGuid()).ToList();

        //      var selectedQuestions = new List<QuestionDto>();
        //      int remainingCount = count;

        //      // Step 2: Randomly select questions from different lectures
        //      var usedLectures = new HashSet<int>();

        //      while (remainingCount > 0 && shuffledLectureIds.Any())
        //      {
        //          int randomIndex = new Random().Next(shuffledLectureIds.Count);
        //          int selectedLectureId = shuffledLectureIds[randomIndex];

        //          // Ensure we don't take multiple questions from the same lecture
        //          if (!usedLectures.Contains(selectedLectureId))
        //          {
        //              var lectureQuestions = await baseQuery
        //                  .Where(q => q.LectureId == selectedLectureId)
        //                  .OrderBy(q => Guid.NewGuid())
        //                  .Take(1)
        //                  .ProjectTo<QuestionDto>(_mapper.ConfigurationProvider)
        //                  .ToListAsync();

        //              if (lectureQuestions.Any())
        //              {
        //                  selectedQuestions.AddRange(lectureQuestions);
        //                  usedLectures.Add(selectedLectureId); // Mark lecture as used
        //                  remainingCount -= lectureQuestions.Count;
        //              }

        //              // Remove lecture from shuffle list to avoid repetition
        //              shuffledLectureIds.RemoveAt(randomIndex);
        //          }
        //      }

        //      return selectedQuestions;
        //  }

        public async Task<List<QuestionDto>> GetQuestionsByTypeAndDifficultyAsync(
    string type,
    string difficulty,
    int count,
    int professorId,
    int? courseId,
    int? categoryId,
    List<int> lectureIds)
        {
            if (count <= 0)
                return new List<QuestionDto>();

            var baseQuery = _context.Questions
                .Include(q => q.QuestionType)
                .Include(q => q.DifficultyLevel)
                .Include(q => q.Lecture)
                .Where(q =>
                    q.QuestionType.Type == type &&
                    q.DifficultyLevel.Level == difficulty &&
                    q.ProfessorId == professorId &&
                    (courseId == null || q.CourseId == courseId) &&
                    (categoryId == null || q.CategoryId == categoryId) &&
                    (lectureIds.Count == 0 || lectureIds.Contains(q.LectureId)));

            // No lecture constraint: random selection
            if (!lectureIds.Any())
            {
                return await baseQuery.OrderBy(q => Guid.NewGuid())
                                      .Take(count)
                                      .ProjectTo<QuestionDto>(_mapper.ConfigurationProvider)
                                      .ToListAsync();
            }

            // Balanced selection
            var selectedQuestions = new List<QuestionDto>();
            int lecturesCount = lectureIds.Count;
            int baseCountPerLecture = count / lecturesCount;
            int remainder = count % lecturesCount;

            var shuffledLectureIds = lectureIds.OrderBy(_ => Guid.NewGuid()).ToList();

            foreach (var lectureId in shuffledLectureIds)
            {
                int questionsNeeded = baseCountPerLecture + (remainder > 0 ? 1 : 0);
                if (remainder > 0) remainder--;

                var lectureQuestions = await baseQuery
                    .Where(q => q.LectureId == lectureId)
                    .OrderBy(q => Guid.NewGuid())
                    .Take(questionsNeeded)
                    .ProjectTo<QuestionDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                selectedQuestions.AddRange(lectureQuestions);
            }

            // Fill if we didn't reach the requested count (e.g. not enough questions per lecture)
            if (selectedQuestions.Count < count)
            {
                int remaining = count - selectedQuestions.Count;
                var extraQuestions = await baseQuery
                    .Where(q => !selectedQuestions.Select(sq => sq.Id).Contains(q.Id))
                    .OrderBy(q => Guid.NewGuid())
                    .Take(remaining)
                    .ProjectTo<QuestionDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                selectedQuestions.AddRange(extraQuestions);
            }

            return selectedQuestions;
        }





    }
}