using BotMain.Core.DataAccess;
using BotMain.Entities;
//using Otus.ToDoList.ConsoleBot.Types;
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

        Task<ToDoUser?> IUserRepository.GetUser(Guid userId, CancellationToken ct)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.UserId == userId));
        }
        Task<ToDoUser?> IUserRepository.GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.TelegramUserId == telegramUserId));
        }
        Task IUserRepository.Add(ToDoUser user, CancellationToken ct)
        {
            if (!_users.Contains(user))
            {
                _users.Add(user);
            }

            return Task.CompletedTask;
        }
      
    
    }
}
