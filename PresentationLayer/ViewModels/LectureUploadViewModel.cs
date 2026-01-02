namespace PresentationLayer.ViewModels
{
    public class LectureUploadViewModel
    {
        public int SelectedCourseId { get; set; }
        public string SelectedCourseName { get; set; } = string.Empty;
        public int SelectedLectureId { get; set; }
        public string SelectedLectureName { get; set; } = string.Empty;
        public IFormFile? File { get; set; }

        public List<CourseOptionViewModel> Courses { get; set; } = new();

    }
   

    public class LectureOptionViewModel
    {
        public int Id { get; set; }
        public string LectureName { get; set; }
        public List<LectureFileOptionViewModel> Files { get; set; } = new();
        public List<string> FileNames { get; set; } = new List<string>(); // new


        public class LectureFileOptionViewModel
        {
            public int Id { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }
    }
}
