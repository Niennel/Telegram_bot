using BotMain.Core.DataAccess;
using BotMain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static BotMain.Entities.ToDoItem;

namespace BotMain.Infrastructure.DataAccess
{
    internal class FileToDoRepository : IToDoRepository
    {
        private readonly string _Path;
        private readonly string _indexPath;
        private Dictionary<Guid, Guid> _itemToUserIndex;
        public FileToDoRepository(string path) 
        {
            if (path==null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            _Path = path;
            _indexPath = Path.Combine(_Path, "_index.json");
            if (!Directory.Exists(_Path))
            {
                Directory.CreateDirectory(_Path);
            }
            _itemToUserIndex = LoadIndexAsync().GetAwaiter().GetResult();
        }
        private string GetUserFolderPath(Guid userId) => Path.Combine(_Path, userId.ToString());
        private string GetFilePath(Guid userId, Guid taskid) => Path.Combine(GetUserFolderPath(userId), $"{taskid}.json");

        private async Task EnsureUserFolderExists(Guid userId)
        {
            var userFolder = GetUserFolderPath(userId);
            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
            }
        }

        private async Task<Dictionary<Guid, Guid>> LoadIndexAsync()
        {
            if (File.Exists(_indexPath))
            {
                await using var indexStream = File.OpenRead(_indexPath);
                return await JsonSerializer.DeserializeAsync<Dictionary<Guid, Guid>>(indexStream)
                    ?? new Dictionary<Guid, Guid>();
            }

            // Если файла индекса нет - строим индекс по существующим данным
            var index = new Dictionary<Guid, Guid>();
            foreach (var userFolder in Directory.GetDirectories(_Path))
            {
                var userId = Guid.Parse(Path.GetFileName(userFolder));
                foreach (var itemFile in Directory.GetFiles(userFolder, "*.json"))
                {
                    var itemId = Guid.Parse(Path.GetFileNameWithoutExtension(itemFile));
                    index[itemId] = userId;
                }
            }
            await SaveIndexAsync(index);
            return index;
        }
        private async Task SaveIndexAsync(Dictionary<Guid, Guid> index)
        {
            await using var indexStream = File.Create(_indexPath);
            await JsonSerializer.SerializeAsync(indexStream, index);
        }
        private async Task<ToDoItem?> GetItemFromFile( string filePath, CancellationToken ct)
        {
            await using var fileStream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<ToDoItem>(fileStream, cancellationToken: ct);
        }
        async Task IToDoRepository.Add(ToDoItem item, CancellationToken ct)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.User == null) throw new ArgumentException("Item must have a User");
            if (item.Id == Guid.Empty) item.Id = Guid.NewGuid();

            await EnsureUserFolderExists(item.User.UserId);
            var path = GetFilePath(item.User.UserId, item.Id);

            await using var fileStream = File.Create(path);
            await JsonSerializer.SerializeAsync(fileStream, item, cancellationToken: ct);

            // Обновляем индекс
            _itemToUserIndex[item.Id] = item.User.UserId;
            await SaveIndexAsync(_itemToUserIndex);
        }

        public async Task<int> CountActive(Guid userId, CancellationToken ct)
        {
            var allActive = await GetActiveByUserId(userId, ct);
            return allActive.Count();
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {

            if (_itemToUserIndex.TryGetValue(id, out var userId))
            { 
            var filePath = GetFilePath(userId, id);
            if (File.Exists(filePath)) 
                File.Delete(filePath);
            }
            // Удаляем из индекса независимо от существования файла
            _itemToUserIndex.Remove(id);
            await SaveIndexAsync(_itemToUserIndex);
        }

        public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            var allFiles = await GetAllByUserId(userId, ct);
            var result = allFiles.Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
            return result;
        }

        public async Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
        {
            var userItems = await GetAllByUserId(userId, ct);
            return userItems.Where(predicate).ToList().AsReadOnly();
        }

        public async Task<ToDoItem?> Get(Guid id, CancellationToken ct)
        {
            //var filePath = GetFilePath(id);
            //var file = File.Exists(filePath);
            //return file? await GetItemFromFile(filePath,ct):null;

            if (_itemToUserIndex.TryGetValue(id, out var userId))
            {
                var filePath = GetFilePath(userId, id);
                if (File.Exists(filePath))
                {
                    await using var fileStream = File.OpenRead(filePath);
                    return await JsonSerializer.DeserializeAsync<ToDoItem>(fileStream, cancellationToken: ct);
                }
                // Если файл не найден, но есть в индексе - чистим индекс
                _itemToUserIndex.Remove(id);
                await SaveIndexAsync(_itemToUserIndex);
            }
            return null;
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
        {
            var allFiles = await GetAllByUserId(userId, ct);
            var result = allFiles.Where(x => x.State == ToDoItemState.Active);
            return result.ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
        {
           var userFolder = GetUserFolderPath(userId);
           if (!Directory.Exists(userFolder)) return Array.Empty<ToDoItem>().AsReadOnly();
           var allFiles = Directory.GetFiles(userFolder, "*.json");
           var result = new List<ToDoItem>();
            foreach (var file in allFiles)
            {
                ct.ThrowIfCancellationRequested();
                var item = await GetItemFromFile(file, ct);
                if(item.User.UserId==userId)
                  result.Add(item);
            }
            return result.AsReadOnly();
        }

        public async Task Update(ToDoItem item, CancellationToken ct)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.User == null) throw new ArgumentException("Item must have a User");

            var path = GetFilePath(item.User.UserId, item.Id);
            if (!File.Exists(path))
                throw new FileNotFoundException("ToDoItem not found", path);

            await using var fileStream = File.Create(path);

            await JsonSerializer.SerializeAsync(fileStream, item, cancellationToken: ct);
        }
    }
}
