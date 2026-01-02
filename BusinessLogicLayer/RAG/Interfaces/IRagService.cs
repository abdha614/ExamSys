using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.RAG.Interfaces
{
    public interface IRagService
    {

        Task CallRagBackendAsync(LectureFile lectureFile, int lectureId, string courseName, string lectureName);
       // Task CallRagBackendBatchAsync(LectureFile[] lectureFiles, int lectureId);
        Task<string> GetRagProcessingStatusAsync(int lectureId);

        Task<string> GenerateQuestionsAsync( List<int> lectureIds, Dictionary<string, int> distribution, string semanticQuery);
    }
}
