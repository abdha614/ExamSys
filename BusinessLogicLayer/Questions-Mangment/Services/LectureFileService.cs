using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Questions_Mangment.Interfaces;
using BusinessLogicLayer.RAG.Interfaces; // Add this
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Questions_Mangment.Services
{
    public class LectureFileService : ILectureFileService
    {
        private readonly ILectureFileRepository _lectureFileRepository;
        private readonly ILectureRepository _lectureRepository;
        private readonly IRagService _ragService; // Inject RAG service

        public LectureFileService(
            ILectureFileRepository lectureFileRepository,
            ILectureRepository lectureRepository,
            IRagService ragService) // Add to constructor
        {
            _lectureFileRepository = lectureFileRepository;
            _lectureRepository = lectureRepository;
            _ragService = ragService;
        }

        public async Task<LectureFile> SaveLectureFileToDiskAsync(
            string courseName,
            string lectureName,
            int courseId,
            int professorId,
            IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            // 1) Get lecture by name
            var lecture = await _lectureRepository.GetByIdAsync(lectureName, courseId, professorId);
            if (lecture == null)
            {
                lecture = new Lecture
                {
                    LectureName = lectureName,
                    CourseId = courseId,
                    ProfessorId = professorId
                };
                await _lectureRepository.AddAsync(lecture);
            }

            int lectureId = lecture.Id;

            // 2) Check if file already exists
            var existingFile = await _lectureFileRepository
                .GetByCourseIdAndProfessorIdAsync(courseId, professorId, file.FileName);
            if (existingFile != null)
                throw new InvalidOperationException($"A file named '{file.FileName}' already exists.");

            // 3) Save file to disk
            string uploadsPath = Path.Combine("wwwroot", "uploads", "courses", courseId.ToString(), "lectures", lectureId.ToString());
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            string physicalPath = Path.Combine(uploadsPath, uniqueFileName);

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 4) Save metadata to DB
            var lectureFile = new LectureFile
            {
                LectureId = lectureId,
                FileName = file.FileName,
                StoredFileName = uniqueFileName,
                FilePath = $"/uploads/courses/{courseId}/lectures/{lectureId}/{uniqueFileName}",
                ContentType = file.ContentType,
                Size = file.Length,
                UploadedAt = DateTime.UtcNow
            };

            await _lectureFileRepository.AddAsync(lectureFile);

            // 5) Send to RAG backend
            await _ragService.CallRagBackendAsync(lectureFile, lectureFile.Id, courseName, lectureName);


            return lectureFile;
        }
        public async Task DeleteLectureFileAsync(int lectureFileId, bool deleteQuestions)
        {
            // 1. Get the file metadata
            var file = await _lectureFileRepository.GetByIdAsync(lectureFileId);
            if (file == null) throw new KeyNotFoundException("File not found.");

            int lectureId = file.LectureId;

            // 2. Physical Cleanup: Delete the actual file from wwwroot
            string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FilePath.TrimStart('/'));
            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }

            // 3. Database Cleanup
            // We pass the IDs to the repository to handle the logic
            await _lectureRepository.DeleteAsync(lectureFileId, lectureId, deleteQuestions);


        }
    }
}
