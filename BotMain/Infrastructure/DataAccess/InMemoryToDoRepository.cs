using BotMain.Core.DataAccess;
using BotMain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static BotMain.Entities.ToDoItem;

namespace BotMain.Infrastructure.DataAccess
{
    class InMemoryToDoRepository : IToDoRepository
    {
        private readonly List<ToDoItem> tasks = new();
        public void Add(ToDoItem item)
        {
            tasks.Add(item);
        }

        public int CountActive(Guid userId)
        {
            var CA = GetActiveByUserId(userId);
            return CA.Count;
        }

        public void Delete(Guid id)
        {
            tasks.RemoveAll(task => task.Id == id);
        }

        public bool ExistsByName(Guid userId, string name)
        {
            var eTask = tasks
              .Where(task => task.User.UserId == userId)         // задачи этого пользователя
              .Where(task => task.Name == name) // только активные задачи
              .ToList();                                        // в виде списка

            return eTask.Count!=0;
        }

        public ToDoItem? Get(Guid id)
        {
            return tasks.FirstOrDefault(x=>x.Id==id);
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            var activeTasks = tasks
              .Where(task => task.User.UserId == userId)         // задачи этого пользователя
              .Where(task => task.State == ToDoItemState.Active) // только активные задачи
              .ToList();                                        // в виде списка

            return activeTasks;
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            var allTasks = tasks
            .Where(task => task.User.UserId == userId)         // задачи этого пользователя
            .ToList();                                        // в виде списка

            return allTasks;
        }

        public void Update(ToDoItem item)
        {
            var idx = tasks.FindIndex(x=>x.Id==item.Id);
            tasks[idx] = item;
        }

        IReadOnlyList<ToDoItem> IToDoRepository.Find(Guid userId, Func<ToDoItem, bool> predicate)
        {
            var FindTasks = tasks
            .Where(task => task.User.UserId == userId)         // задачи этого пользователя
            .Where(task => predicate(task))                   //которые удовлетворяют предикату
            .ToList();                                        // в виде списка

            return FindTasks;
        }
    }
}
