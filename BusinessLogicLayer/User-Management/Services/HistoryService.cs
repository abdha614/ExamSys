using AutoMapper;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly IMapper _mapper;
        public HistoryService(IHistoryRepository historyRepository,IMapper mapper)
        {
            _historyRepository = historyRepository;
            _mapper = mapper;
        }
       // Inject IMapper to use AutoMapper
      //  Get history data for display in the admin panel
        public async Task<List<HistoryDto>> GetHistoryAsync()
        {
            // Fetch all history data (this can be customized with filtering)
            var historyEntities = await _historyRepository.GetAllHistoryAsync();

            // Map entities to DTOs for easier transfer
            var historyDtos = _mapper.Map<List<HistoryDto>>(historyEntities);

            return historyDtos;
        }
        public async Task LogActionAsync(HistoryDto historyDto)
        {
            // Set the actual time (UTC now)
            historyDto.Timestamp = DateTime.Now;
            // Map the HistoryDto to a History entity
            var history = new History
            {
                UserId = historyDto.UserId,
                Action = historyDto.Action,
                Timestamp = historyDto.Timestamp,
                IpAddress = historyDto.IpAddress // Store the IP address in the History record
            };

            // Log the history entry to the database
            await _historyRepository.AddHistoryAsync(history);
        }


    }
}
