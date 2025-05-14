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
    
        async Task <ToDoUser?> IUserService.GetUser(long telegramUserId, CancellationToken ct)
        {
            return await userRepository.GetUserByTelegramUserId(telegramUserId,ct);
        }

        public async Task <ToDoUser> RegisterUser(long telegramUserId, string telegramUserName, CancellationToken ct)
        {
            var newUser = new ToDoUser()
            {
                RegisteredAt = DateTime.Now,
                TelegramUserId = telegramUserId,
                TelegramUserName = telegramUserName,
                UserId = Guid.NewGuid()
            };

            await userRepository.Add(newUser, ct);
            return newUser;
        }
    }
}
