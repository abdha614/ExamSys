using System;
using System.Collections.Generic;

namespace PresentationLayer.ViewModels
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string QuestionTypeName { get; set; }
        public string DifficultyLevelName { get; set; }
      //  public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CategoryName { get; set; }
        public string CourseName { get; set; }
    }
}
