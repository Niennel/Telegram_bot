using System;
using System.Text;
using System.Text.Unicode;
using static BotMain.UpdateHandler;
using static BotMain.Exceptions.TaskCountLimitException;

using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using BotMain.Core.DataAccess;
using BotMain.Entities;
using BotMain.Infrastructure.DataAccess;
using BotMain.Services;

namespace BotMain
{
    internal class Program
    {
        public static int maxTasks;
        public static int maxTaskLength;

        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            var botClient = new ConsoleBotClient();

            IUserRepository userRepository = new InMemoryUserRepository();
            IToDoRepository toDoRepository = new InMemoryToDoRepository();
            IToDoReportService toDoReportService = new ToDoReportService(toDoRepository);
            var userService = new UserService(userRepository);
            var toDoService = new ToDoService(toDoRepository);
            var handler = new UpdateHandler(userService, toDoService,toDoReportService);

            try
            {
                Console.Write("Введите максимально допустимое количество задач: ");
                maxTasks = ParseAndValidateInt(Console.ReadLine(), 1, 100);

                Console.Write("Введите максимально допустимую длину задачи: ");
                maxTaskLength = ParseAndValidateInt(Console.ReadLine(), 1, 100);

                botClient.StartReceiving(handler);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            
        }
       
        private static int ParseAndValidateInt(string? str, int min, int max)
        {
            var isNumber = int.TryParse(str, out var number);
            if (!isNumber || number < min || number > max)
                throw new ArgumentException();
            return number;
        }

        internal static string ValidateString(string? str)
        {
            if (str != null && str.Replace(" ", "").Replace("\t", "").Length > 0)
                return str;
            throw new ArgumentException();
        }
    }
}
