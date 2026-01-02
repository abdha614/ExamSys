using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.LectureDto
{
    public class LectureGetDto
    {
        public int Id { get; set; }
        public string LectureName { get; set; }
        public List<string> FileNames { get; set; } = new List<string>();
        public List<LectureFileGetDto> Files { get; set; } = new();
    }

}
