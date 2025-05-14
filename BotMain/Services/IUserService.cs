using BotMain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static BotMain.UpdateHandler;
using static BotMain.Entities.ToDoUser;

namespace BotMain.Services
{
    public interface IUserService
    {
        Task<ToDoUser> RegisterUser(long telegramUserId, string telegramUserName, CancellationToken ct);
        Task<ToDoUser?> GetUser(long telegramUserId, CancellationToken ct);
    }
}
