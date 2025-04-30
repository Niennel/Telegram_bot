using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BotMain.UpdateHandler;
using static BotMain.ToDoUser;


namespace BotMain
{
    class UserService : IUserService
    {
        private readonly List<ToDoUser> _users = new();
     
        ToDoUser? IUserService.GetUser(long telegramUserId)
        {
            return _users.FirstOrDefault(u => u.TelegramUserId == telegramUserId);
        }

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            foreach (var user in _users.Where(user => user.TelegramUserId == telegramUserId))
            {
                //пользователь уже зарегистрирован
                return user;
            }

            var newUser = new ToDoUser()
            {
                RegisteredAt = DateTime.Now,
                TelegramUserId = telegramUserId,
                TelegramUserName = telegramUserName,
                UserId = Guid.NewGuid()
            };
            
            _users.Add(newUser);
            return newUser;
        }
    }
}
