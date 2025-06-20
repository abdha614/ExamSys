﻿using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IAnswerRepository : IGenericRepository<Answer>
    {
        // Add specific methods related to answers
        Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(int questionId);
     

    }
}
