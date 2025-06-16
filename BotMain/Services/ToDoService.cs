//using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static BotMain.Entities.ToDoItem;
using BotMain.Exceptions;
using BotMain.Entities;

namespace BotMain.Services
{
    class ToDoService(Core.DataAccess.IToDoRepository tasks) : IToDoService
    {
        public async Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix, CancellationToken ct)
        {
            string _Prefix;
            _Prefix = Program.ValidateString(namePrefix);

            return await tasks.Find(user.UserId, item => item.Name.StartsWith(namePrefix), ct);
        }

        //private readonly List<ToDoItem> tasks = new();
        public async Task<ToDoItem > Add(ToDoUser user, string name, DateTime? deadline, CancellationToken ct)
        {
            //проверка на количество
            var count = await tasks.CountActive(user.UserId, ct);
            if (count >= Program.maxTasks)
                throw new TaskCountLimitException(Program.maxTasks);
            
            string task_in;
            task_in = Program.ValidateString(name);
            
            //проверка на длинну
            if (task_in.Length > Program.maxTaskLength)
                throw new TaskLengthLimitException(task_in.Length, Program.maxTaskLength);

            var newTask = new ToDoItem(task_in, user)
            {
                Deadline = deadline
            };

            //проверка на наличие
            if (await  tasks.ExistsByName(user.UserId, name, ct))
                throw new DuplicateTaskException(task_in);
            
            await tasks.Add(newTask,ct);

            return newTask;
        }

        async Task IToDoService.Delete(Guid id, CancellationToken ct)
        {
            await tasks.Delete(id, ct);
        }

         async Task <IReadOnlyList<ToDoItem>> IToDoService.GetActiveByUserId(Guid userId, CancellationToken ct)
        {
            return await tasks.GetActiveByUserId(userId, ct);
        }

        async Task<IReadOnlyList<ToDoItem>> IToDoService.GetAllByUserId(Guid userId, CancellationToken ct)
        {
            return await tasks.GetAllByUserId(userId, ct);
        }

        async Task IToDoService.MarkCompleted(Guid id, CancellationToken ct)
        {
            var task = await tasks.Get(id, ct);
            if (task == null) return;
                task.State = ToDoItemState.Completed;
                task.StateChangedAt = DateTime.Now;

            await tasks.Update(task, ct);
        }
    }
}
