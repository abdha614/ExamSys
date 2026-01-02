using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ILectureRepository : IGenericRepository<Lecture>
    {
        Task<Lecture> GetByIdAsync(string lectureName, int courseId, int professorId); // Fetch lecture by ID
        Task DeleteAsync(int lectureFileId, int lectureId, bool deleteQuestions);


    }
}
