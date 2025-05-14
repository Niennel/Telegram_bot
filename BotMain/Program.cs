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
        private static void DisplayMessageStart(string message)
        {
            Console.WriteLine($"Началась обработка сообщения '{message}' в {DateTime.Now}");
        }

        private static void DisplayMessageStop(string message)
        {
            Console.WriteLine($"Закончилась обработка сообщения '{message}' в {DateTime.Now}");
        }


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
            using var cts = new CancellationTokenSource();
            var handler = new UpdateHandler(userService, toDoService,toDoReportService, cts);

            handler.OnHandleUpdateStarted += DisplayMessageStart;//подписываемся
            handler.OnHandleUpdateCompleted += DisplayMessageStop;//подписываемся
            try
            {
                Console.Write("Введите максимально допустимое количество задач: ");
                maxTasks = ParseAndValidateInt(Console.ReadLine(), 1, 100);

                Console.Write("Введите максимально допустимую длину задачи: ");
                maxTaskLength = ParseAndValidateInt(Console.ReadLine(), 1, 100);

                botClient.StartReceiving(handler, cts.Token);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            finally
            {
                handler.OnHandleUpdateStarted -= DisplayMessageStart; //отписываемся
                handler.OnHandleUpdateCompleted -= DisplayMessageStop;//отписываемся
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
