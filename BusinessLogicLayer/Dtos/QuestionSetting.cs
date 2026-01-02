using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos
{
    public class QuestionSetting
    {
        public int CategoryId { get; set; }
        public int CourseId { get; set; }
        public string LectureName { get; set; }
        public IFormFile PdfFile { get; set; }

        public int McqEasy { get; set; }
        public int McqMedium { get; set; }
        public int McqHard { get; set; }

        public int TfEasy { get; set; }
        public int TfMedium { get; set; }
        public int TfHard { get; set; }
    }
}
