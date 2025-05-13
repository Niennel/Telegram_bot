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

        ToDoUser? IUserRepository.GetUser(Guid userId)
        {
            return _users.FirstOrDefault(u => u.UserId == userId);
        }
        ToDoUser? IUserRepository.GetUserByTelegramUserId(long telegramUserId)
        {
            return _users.FirstOrDefault(u => u.TelegramUserId == telegramUserId);
        }
        void IUserRepository.Add(ToDoUser user)
        {
            if(!_users.Contains(user))
            {
                _users.Add(user);
            }
        }

      
    
    }
}
