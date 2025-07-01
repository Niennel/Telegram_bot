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
        public Task Add(ToDoItem item,  CancellationToken ct)
        {
            tasks.Add(item);
            return Task.CompletedTask;
        }

        public async Task<int> CountActive(Guid userId, CancellationToken ct)
        {
            var CA = await GetActiveByUserId(userId,  ct);
            return CA.Count;
        }

        public Task Delete(Guid id, CancellationToken ct)
        {
            tasks.RemoveAll(task => task.Id == id);
            return Task.CompletedTask;
        }

        public Task <bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            var eTask = tasks
              .Where(task => task.User.UserId == userId)         // задачи этого пользователя
              .Where(task => task.Name == name) // только активные задачи
              .ToList();                                        // в виде списка

            return Task.FromResult(eTask.Count!=0);
        }

        public Task<ToDoItem?> Get(Guid id, CancellationToken ct)
        {
            return Task.FromResult(tasks.FirstOrDefault(x=>x.Id==id));
        }

        public Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
        {
            var activeTasks = tasks
              .Where(task => task.User.UserId == userId)         // задачи этого пользователя
              .Where(task => task.State == ToDoItemState.Active) // только активные задачи
              .ToList();                                        // в виде списка

            return Task.FromResult<IReadOnlyList<ToDoItem>>(activeTasks);
        }

        public Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
        {
            var allTasks = tasks
            .Where(task => task.User.UserId == userId)         // задачи этого пользователя
            .ToList();                                        // в виде списка

            return Task.FromResult<IReadOnlyList<ToDoItem>>(allTasks);
        }

        public Task Update(ToDoItem item, CancellationToken ct)
        {
            var idx = tasks.FindIndex(x=>x.Id==item.Id);
            tasks[idx] = item;
            return Task.CompletedTask;
        }

        Task <IReadOnlyList<ToDoItem>> IToDoRepository.Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
        {
            var FindTasks = tasks
            .Where(task => task.User.UserId == userId)         // задачи этого пользователя
            .Where(task => predicate(task))                   //которые удовлетворяют предикату
            .ToList();                                        // в виде списка

            return Task.FromResult<IReadOnlyList<ToDoItem>>(FindTasks);
        }
    }
}
