using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public HistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddHistoryAsync(History history)
        {
            await _context.Histories.AddAsync(history);
            await _context.SaveChangesAsync();
        }
      //  Get all history logs
        public async Task<List<History>> GetAllHistoryAsync()
        {
            // Fetch all the history records from the Histories table
            var historyRecords = await _context.Histories
                                                .Include(h => h.User)
                                                .OrderByDescending(h => h.Timestamp) // Optional: Sort by timestamp
                                                .ToListAsync();
            return historyRecords;
        }

    }
}
