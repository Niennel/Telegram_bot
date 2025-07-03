using BotMain.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMain.Entities
{
    public class ToDoListService : IToDoListService
    {
        public readonly IToDoListRepository _toDoListRepository;
        public ToDoListService(IToDoListRepository toDoListRepository)
        {
            // Проверяем, что зависимость не является null
            if (toDoListRepository == null)
            {
                throw new ArgumentNullException(
                    paramName: nameof(toDoListRepository),
                    message: "Репозиторий списков задач не может быть null");
            }

            // Инициализируем поле класса
            _toDoListRepository = toDoListRepository;
        }
        public async Task<ToDoList> Add(ToDoUser user, string name, CancellationToken ct)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("List name cannot be empty", nameof(name));

            if (name.Length > 10)
                throw new ArgumentException("List name cannot be longer than 10 characters", nameof(name));

            if (await _toDoListRepository.ExistsByName(user.UserId, name, ct))
                throw new InvalidOperationException($"List with name '{name}' already exists for this user");

            var newList = new ToDoList
            {
                Id = Guid.NewGuid(),
                User = user,
                Name = name,
                CreatedAt = DateTime.UtcNow
            };

            await _toDoListRepository.Add(newList, ct);
            return newList;
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            await _toDoListRepository.Delete(id, ct);
        }

        public async Task<ToDoList?> Get(Guid id, CancellationToken ct)
        {
            return await _toDoListRepository.Get(id, ct);
        }

        public async Task<IReadOnlyList<ToDoList>> GetUserLists(Guid userId, CancellationToken ct)
        {
            return await _toDoListRepository.GetByUserId(userId, ct);
        }
    }
}
