using BotMain.Core.DataAccess;
using BotMain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotMain.Infrastructure.DataAccess
{
    internal class FileToDoListRepository : IToDoListRepository
    {
        private readonly string _Path;

        public FileToDoListRepository(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            _Path = path;

            if (!Directory.Exists(_Path))
            {
                Directory.CreateDirectory(_Path);
            }
        }
        private string GetFilePath(Guid id) => Path.Combine(_Path, $"{id}.json");

        public async Task Add(ToDoList list, CancellationToken ct)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            var filePath = GetFilePath(list.Id);
            if (File.Exists(filePath))
            {
                throw new InvalidOperationException($"User with id {list.Id} already exists");

            }
            await using var fileStream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(fileStream, list, cancellationToken: ct);
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            var filePath = GetFilePath(id);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            var listFiles = Directory.GetFiles(_Path, "*.json");
            foreach (var file in listFiles)
            {
                await using var fileStream = File.OpenRead(file);
                var list = await JsonSerializer.DeserializeAsync<ToDoList>(fileStream, cancellationToken: ct);
                if (list?.User.UserId == userId && list.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<ToDoList?> Get(Guid id, CancellationToken ct)
        {
            var filePath = GetFilePath(id);
            var file = File.Exists(filePath);
            if (!file)
            {
                return null;
            }
            await using var fileStream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<ToDoList>(fileStream, cancellationToken: ct);
        }

        public async Task<IReadOnlyList<ToDoList>> GetByUserId(Guid userId, CancellationToken ct)
        {
            var userFiles = Directory.GetFiles(_Path, "*.json");
            var userLists = new List<ToDoList>();
            foreach (var file in userFiles) 
            {
                await using var fileStream = File.OpenRead(file);
                var list = await JsonSerializer.DeserializeAsync<ToDoList>(fileStream, cancellationToken: ct);
                if (list.Name != null)
                if (list?.User.UserId == userId)
                {
                    userLists.Add(list);
                }
            }
            return userLists.AsReadOnly();
        }
    }
}
