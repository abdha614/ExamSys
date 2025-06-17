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
        public async Task<Lecture> GetByIdAsync(string LectureName, int courseId)
        {
            var result = await _dbSet.FirstOrDefaultAsync(l => l.LectureName == LectureName && l.CourseId == courseId);
            return result;
        }


    }
}
