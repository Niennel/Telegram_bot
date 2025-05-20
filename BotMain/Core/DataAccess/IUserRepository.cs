using BotMain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMain.Core.DataAccess
{
    interface IUserRepository
    {
        Task<ToDoUser?> GetUser(Guid userId, CancellationToken ct);
        Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct);
        Task Add(ToDoUser user, CancellationToken ct);
    }
}
