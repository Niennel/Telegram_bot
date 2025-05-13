using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BotMain.UpdateHandler;
using static BotMain.Entities.ToDoUser;
using BotMain.Entities;


namespace BotMain.Services
{
    class UserService (Core.DataAccess.IUserRepository userRepository) : IUserService
    {
    
        ToDoUser? IUserService.GetUser(long telegramUserId)
        {
            return userRepository.GetUserByTelegramUserId(telegramUserId);
        }

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            var newUser = new ToDoUser()
            {
                RegisteredAt = DateTime.Now,
                TelegramUserId = telegramUserId,
                TelegramUserName = telegramUserName,
                UserId = Guid.NewGuid()
            };

            userRepository.Add(newUser);
            return newUser;
        }
    }
}
