using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ILectureFileRepository : IGenericRepository<LectureFile>
    {
        // Task<LectureFile?> GetByHashAsync(string fileHash);
        Task<LectureFile?> GetByCourseIdAndProfessorIdAsync(int courseId, int professorId, string fileName);
    }
}
