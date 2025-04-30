using System;
using System.Text;
using System.Text.Unicode;
using static BotMain.UpdateHandler;
using static BotMain.TaskCountLimitException;

 using Otus.ToDoList.ConsoleBot;
 using Otus.ToDoList.ConsoleBot.Types;

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
            var userService = new UserService();
            var toDoService = new ToDoService();
            var handler = new UpdateHandler(userService, toDoService);

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
