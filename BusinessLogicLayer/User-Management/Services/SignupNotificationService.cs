using AutoMapper;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.User_Management.Interfaces;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.User_Management.Services
{
    public class SignupNotificationService : ISignupNotificationService
    {
        private readonly ISignupNotificationRepository _notificationRepo;
        private readonly IMapper _mapper;

        public SignupNotificationService(ISignupNotificationRepository notificationRepo, IMapper mapper)
        {
            _notificationRepo = notificationRepo;
            _mapper = mapper;
        }

        public async Task SaveSignupRequestAsync(string email)
        {
            var exists = await _notificationRepo.ExistsUnHandledEmailAsync(email);
            if (!exists)
            {
                var timeNow = DateTime.Now;
                var notification = new SignupNotification
                {
                    Email = email,
                    RequestedAt = timeNow,
                    IsHandled = false
                };
                await _notificationRepo.AddAsync(notification);
            }
        }

        public async Task<List<SignupNotificationDto>> GetUnhandledRequestsAsync()
        {
            var unhandled = await _notificationRepo.GetAllUnhandledAsync();
            return _mapper.Map<List<SignupNotificationDto>>(unhandled);
        }

        public async Task MarkAsHandledAsync(int id)
        {
            var request = await _notificationRepo.GetByIdAsync(id);
            if (request != null && !request.IsHandled)
            {
                request.IsHandled = true;
                await _notificationRepo.UpdateAsync(request);
            }
        }
        public async Task MarkAsHandledByEmailAsync(string email)
        {
            var request = await _notificationRepo.GetUnhandledByEmailAsync(email);
            if (request != null)
            {
                request.IsHandled = true;
                await _notificationRepo.UpdateAsync(request);
            }
        }
        public async Task<List<SignupNotificationDto>> GetAllRequestsAsync()
        {
            var all = await _notificationRepo.GetAlllAsync();
            return _mapper.Map<List<SignupNotificationDto>>(all);
        }

    }
}
