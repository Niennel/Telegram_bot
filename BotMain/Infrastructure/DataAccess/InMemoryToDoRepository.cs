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
        public async Task Add(ToDoItem item, CancellationToken ct)
        {
            tasks.Add(item);
        }

        public async Task<int> CountActive(Guid userId, CancellationToken ct)
        {
            var CA = await GetActiveByUserId(userId,  ct);
            return CA.Count;
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            tasks.RemoveAll(task => task.Id == id);
        }

        public async Task <bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            var eTask = tasks
              .Where(task => task.User.UserId == userId)         // задачи этого пользователя
              .Where(task => task.Name == name) // только активные задачи
              .ToList();                                        // в виде списка

            return eTask.Count!=0;
        }

        public async Task<ToDoItem?> Get(Guid id, CancellationToken ct)
        {
            return tasks.FirstOrDefault(x=>x.Id==id);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
        {
            var activeTasks = tasks
              .Where(task => task.User.UserId == userId)         // задачи этого пользователя
              .Where(task => task.State == ToDoItemState.Active) // только активные задачи
              .ToList();                                        // в виде списка

            return activeTasks;
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
        {
            var allTasks = tasks
            .Where(task => task.User.UserId == userId)         // задачи этого пользователя
            .ToList();                                        // в виде списка

            return allTasks;
        }

        public async Task Update(ToDoItem item, CancellationToken ct)
        {
            var idx = tasks.FindIndex(x=>x.Id==item.Id);
            tasks[idx] = item;
        }

        async Task <IReadOnlyList<ToDoItem>> IToDoRepository.Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
        {
            var FindTasks = tasks
            .Where(task => task.User.UserId == userId)         // задачи этого пользователя
            .Where(task => predicate(task))                   //которые удовлетворяют предикату
            .ToList();                                        // в виде списка

            return FindTasks;
        }
    }
}
