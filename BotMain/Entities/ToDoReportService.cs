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
       public (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId)
        {
            var total = toDoRepository.GetAllByUserId(userId).Count();
            var active = toDoRepository.CountActive(userId);
            var completed = total - active;

            return (total,completed,active, generatedAt: DateTime.Now);
        }
    }
}
