using DataAccessLayer.Data;
using DataAccessLayer.Dtos;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class LectureRepository : GenericRepository<Lecture>, ILectureRepository
    {
        public LectureRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Fetch Lecture by ID
        public async Task<Lecture> GetByIdAsync(string lectureName, int courseId, int professorId)
        {
            var result = await _dbSet
                .FirstOrDefaultAsync(l =>
                    l.LectureName == lectureName &&
                    l.CourseId == courseId &&
                    l.ProfessorId == professorId);

            return result;
        }
        public async Task DeleteAsync(int lectureFileId, int lectureId, bool deleteQuestions)
        {
            // 1. Find the specific file
            var lectureFile = await _context.LectureFiles
                .FirstOrDefaultAsync(lf => lf.Id == lectureFileId);

            if (lectureFile == null) return;

            // 2. Handle Questions/Answers only if requested
           
            // 3. Delete ONLY the LectureFile
            // If deleteQuestions was false, we only hit this line.
            _context.LectureFiles.Remove(lectureFile);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // If this hits, it means a Foreign Key is pointing DIRECTLY at the File ID
                throw new Exception("Cannot delete file record because it is referenced elsewhere: " + ex.Message);
            }
        }




    }
}
