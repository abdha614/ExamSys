using BusinessLogicLayer.Dtos;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IHistoryService
    {
        Task LogActionAsync(HistoryDto historyDto);
        Task<List<HistoryDto>> GetHistoryAsync();



    }
}
