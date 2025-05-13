using Otus.ToDoList.ConsoleBot.Types;
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
        public IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix)
        {
            string _Prefix;
            _Prefix = Program.ValidateString(namePrefix);

            return tasks.Find(user.UserId, item => item.Name.StartsWith(namePrefix));
        }

        //private readonly List<ToDoItem> tasks = new();
        ToDoItem IToDoService.Add(ToDoUser user, string name)
        {
            //проверка на количество
            if (tasks.CountActive(user.UserId)>=Program.maxTasks)
                throw new TaskCountLimitException(Program.maxTasks);
            
            string task_in;
            task_in = Program.ValidateString(name);
            
            //проверка на длинну
            if (task_in.Length > Program.maxTaskLength)
                throw new TaskLengthLimitException(task_in.Length, Program.maxTaskLength);
           
            var newTask = new ToDoItem(task_in,user);

            //проверка на наличие
            if (tasks.ExistsByName(user.UserId,name))
                throw new DuplicateTaskException(task_in);
            
            tasks.Add(newTask);

            return newTask;
        }

        void IToDoService.Delete(Guid id)
        {
            tasks.Delete(id);
        }

        IReadOnlyList<ToDoItem> IToDoService.GetActiveByUserId(Guid userId)
        {
            return tasks.GetActiveByUserId(userId);
        }

        IReadOnlyList<ToDoItem> IToDoService.GetAllByUserId(Guid userId)
        {
            return tasks.GetAllByUserId(userId);
        }

        void IToDoService.MarkCompleted(Guid id)
        {
            var task = tasks.Get(id);
            if (task == null) return;
                task.State = ToDoItemState.Completed;
                task.StateChangedAt = DateTime.Now;

            tasks.Update(task);
        }
    }
}
