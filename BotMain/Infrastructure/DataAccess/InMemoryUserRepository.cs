using BotMain.Core.DataAccess;
using BotMain.Entities;
using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMain.Infrastructure.DataAccess
{
    class InMemoryUserRepository : IUserRepository
    {
        private readonly List<ToDoUser> _users = [];

        async Task<ToDoUser?> IUserRepository.GetUser(Guid userId, CancellationToken ct)
        {
            return _users.FirstOrDefault(u => u.UserId == userId);
        }
        async Task<ToDoUser?> IUserRepository.GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
        {
            return _users.FirstOrDefault(u => u.TelegramUserId == telegramUserId);
        }
        async Task IUserRepository.Add(ToDoUser user, CancellationToken ct)
        {
            if(!_users.Contains(user))
            {
                _users.Add(user);
            }
        }
      
    
    }
}
