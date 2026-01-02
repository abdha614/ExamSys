using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Dtos.LectureDto;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.ViewModels
{
    public class QuestionSettingsViewModel
    {
        // Selected course ID
        public int CourseId { get; set; }

        // Question counts (Distribution mode or Semantic mode)
        // Keys can be: McqEasy, McqMedium, McqHard, TfEasy, TfMedium, TfHard
        // or SemanticMcqEasy, SemanticMcqMedium, etc.
        public Dictionary<string, int> Distribution { get; set; } = new Dictionary<string, int>();

        // Selected lectures
        public List<LectureSelectionViewModel> LecturesToProcess { get; set; } = new List<LectureSelectionViewModel>();
        public List<FileSelectionViewModel> FilesToProcess { get; set; } = new List<FileSelectionViewModel>();

        // Semantic query (optional)
        public string SemanticQuery { get; set; } = string.Empty;
    }

    public class LectureSelectionViewModel
    {
        public int LectureId { get; set; }
    }
    public class FileSelectionViewModel
    {
        // Use FileId instead of LectureId
        public int FileId { get; set; }
    }
}
