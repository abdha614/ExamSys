using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Questions_Mangment.Interfaces
{
    public interface ILectureFileService
    {

        //   Task<LectureFile> EnsureUploadedAsync(int lectureId, Stream fileStream, string fileName, string contentType);
        Task<LectureFile> SaveLectureFileToDiskAsync(
                  string courseName,
                  string lectureName,
                  int courseId,
                  int professorId,
                  IFormFile file);

        Task DeleteLectureFileAsync(int lectureFileId, bool deleteQuestions);

    }

}
