using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class DifficultyLevel
    {
        public int Id { get; set; }
        public string Level { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}
