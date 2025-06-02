using BotMain.Core.DataAccess;
using BotMain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotMain.Infrastructure.DataAccess
{
    internal class FileUserRepository : IUserRepository
    {
        private readonly string _Path;
        public FileUserRepository(string path)
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
        private string GetFilePath(Guid userid) => Path.Combine(_Path, $"{userid}.json");
        public async Task Add(ToDoUser user, CancellationToken ct)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var filePath = GetFilePath(user.UserId);
            if (File.Exists(filePath))
            {
                throw new InvalidOperationException($"User with id {user.UserId} already exists");
                
            }
            await using var fileStream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(fileStream, user, cancellationToken: ct);
        }

        public async Task<ToDoUser?> GetUser(Guid userId, CancellationToken ct)
        {
            var filePath = GetFilePath(userId);
            var file = File.Exists(filePath);
            if (!file) 
                {
                return null;
                }
            await using var fileStream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<ToDoUser>(fileStream, cancellationToken: ct);
        }

        public async Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
        {
            var userFiles = Directory.GetFiles(_Path, "*.json");
            foreach (var userFile in userFiles) 
            {
                await using var fileStream = File.OpenRead(userFile);
                var user = await JsonSerializer.DeserializeAsync<ToDoUser>(fileStream, cancellationToken: ct);
                if (user?.TelegramUserId == telegramUserId)
                { 
                    return user;
                }
            }
            return null;
        }
    }
}
