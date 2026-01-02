using BusinessLogicLayer.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Questions_Mangment.Interfaces
{
    public interface IQuestionParser
    {
        ParsedQuestionsDto Parse(string aiResponse);
       

    }
}
