using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BotMain.ToDoItem;

namespace BotMain
{
    class ToDoService : IToDoService
    {
        private readonly List<ToDoItem> tasks = new();
        ToDoItem IToDoService.Add(ToDoUser user, string name)
        {
            //проверка на количество
            if (tasks.Count>=Program.maxTasks)
                throw new TaskCountLimitException(Program.maxTasks);
            
            string task_in;
            task_in = Program.ValidateString(name);
            
            //проверка на длинну
            if (task_in.Length > Program.maxTaskLength)
                throw new TaskLengthLimitException(task_in.Length, Program.maxTaskLength);
           
            var newTask = new ToDoItem(task_in,user);

            //проверка на наличие
            var activeTasks = tasks
              .Where(task => task.Name == task_in)         // задачи этого пользователя
              .ToList();
            if (activeTasks.Count>0)
                throw new DuplicateTaskException(task_in);
            
            tasks.Add(newTask);

            return (newTask);
        }

        void IToDoService.Delete(Guid id)
        {
            tasks.RemoveAll(task => task.Id == id);
        }

        IReadOnlyList<ToDoItem> IToDoService.GetActiveByUserId(Guid userId)
        {
            var activeTasks = tasks
              .Where(task => task.User.UserId == userId)         // задачи этого пользователя
              .Where(task => task.State == ToDoItemState.Active) // только активные задачи
              .ToList();                                        // в виде списка
            
            return activeTasks;
        }

        IReadOnlyList<ToDoItem> IToDoService.GetAllByUserId(Guid userId)
        {
            var allTasks = tasks
            .Where(task => task.User.UserId == userId)         // задачи этого пользователя
            .ToList();                                        // в виде списка

            return allTasks;
        }

        void IToDoService.MarkCompleted(Guid id)
        {
            foreach (var task in tasks.Where(task => task.Id == id))
                task.State = ToDoItemState.Completed;
        }
    }
}
