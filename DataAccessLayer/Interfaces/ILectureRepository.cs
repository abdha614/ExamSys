using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ILectureRepository : IGenericRepository<Lecture>
    {
        Task<Lecture> GetByIdAsync(string LectureName, int courseId); // Fetch lecture by ID
    }
}
