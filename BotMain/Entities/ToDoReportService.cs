using BotMain.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMain.Entities
{
    class ToDoReportService(IToDoRepository toDoRepository) : IToDoReportService
    {
       public async Task<(int total, int completed, int active, DateTime generatedAt)> GetUserStats(Guid userId, CancellationToken ct)
        {
            var _total = await toDoRepository.GetAllByUserId(userId,  ct);
            var total = _total.Count;
            var active = await toDoRepository.CountActive(userId, ct);
            var completed = total - active;

            return (total,completed,active, generatedAt: DateTime.Now);
        }
    }
}
