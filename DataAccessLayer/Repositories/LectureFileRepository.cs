using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class LectureFileRepository : GenericRepository<LectureFile>, ILectureFileRepository
    {
        public LectureFileRepository(ApplicationDbContext context) : base(context) { }

        //public async Task<LectureFile?> GetByHashAsync(string fileHash)
        //{
        //    return await _dbSet.FirstOrDefaultAsync(f => f.FileHash == fileHash);
        //}
        public async Task<LectureFile?> GetByCourseIdAndProfessorIdAsync(int courseId, int professorId, string fileName)
        {
            return await _dbSet
                .Include(f => f.Lecture) // optional, in case you need lecture details
                .Where(f => f.Lecture.CourseId == courseId
                            && f.Lecture.ProfessorId == professorId
                            && f.FileName == fileName)
                .FirstOrDefaultAsync();
        }

    }
}
