using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IQuestionImportService
    {
        Task ImportQuestionsAsync(Stream importStream, string fileName, int professorId);
        bool CanHandle(string fileExtension); // New method to check if the service supports the file type
        string GenerateTemplate(string templateType); // Shared template generation method
    }
}
