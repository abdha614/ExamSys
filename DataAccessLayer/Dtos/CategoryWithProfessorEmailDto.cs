using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dtos
{
    public class CategoryWithProfessorEmailDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string ProfessorEmail { get; set; }
        public int ProfessorId { get; set; }
    }
}
