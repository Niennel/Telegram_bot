using System;
using System.Text;
using System.Text.Unicode;
using static BotMain.UpdateHandler;
using static BotMain.Exceptions.TaskCountLimitException;

//using Otus.ToDoList.ConsoleBot;
//using Otus.ToDoList.ConsoleBot.Types;
using BotMain.Core.DataAccess;
using BotMain.Entities;
using BotMain.Infrastructure.DataAccess;
using BotMain.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using BotMain.Scenarios;

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


        public static async Task Main(string[] args)
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            // Получение пути к папке данных приложения
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // Создание подпапки для ZiLong
            string appFolder = Path.Combine(appDataPath, "ZiLong");

            ////Каталоги для задач и пользователей
            //string ToDoFolder = Path.Combine(appFolder, "ToDoFolder");
            //string UserFolder = Path.Combine(appFolder, "UserFolder");


            IUserRepository userRepository = new FileUserRepository(appFolder);
            IToDoRepository toDoRepository = new FileToDoRepository(appFolder);
            IToDoReportService toDoReportService = new ToDoReportService(toDoRepository);
            InMemoryScenarioContextRepository scenarioContext = new InMemoryScenarioContextRepository();
         


            var userService = new UserService(userRepository);
            var toDoService = new ToDoService(toDoRepository);
            var scenarios = new List<IScenario>
             {
                 new AddTaskScenario(userService, toDoService)
             };

            var handler = new UpdateHandler(
                userService,
                toDoService,
                toDoReportService,
                scenarios,  // Передаем как IEnumerable
                scenarioContext);

            try
            {
                string token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN_EX1", EnvironmentVariableTarget.User);
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Bot token not found. Please set the TELEGRAM_BOT_TOKEN_EX1 environment variable.");
                    return;
                }

                var botClient = new TelegramBotClient(token!);
                using var cts = new CancellationTokenSource();

                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = [UpdateType.Message],
                    DropPendingUpdates = true
                };

                handler.OnHandleUpdateStarted += DisplayMessageStart;//подписываемся
                handler.OnHandleUpdateCompleted += DisplayMessageStop;//подписываемся

                Console.Write("Введите максимально допустимое количество задач: ");
                maxTasks = ParseAndValidateInt(Console.ReadLine(), 1, 100);

                Console.Write("Введите максимально допустимую длину задачи: ");
                maxTaskLength = ParseAndValidateInt(Console.ReadLine(), 1, 100);

                botClient.StartReceiving(handler, receiverOptions, cancellationToken: cts.Token);
                var me = await botClient.GetMe();
                Console.WriteLine($"{me.FirstName} запущен!");
                //await Task.Delay(-1); // Устанавливаем бесконечную задержку

                Console.WriteLine("Нажмите клавишу A для выхода");
                while (true)
                {
                    var symbol = Console.ReadKey();
                    if (symbol.Key == ConsoleKey.A)
                    {
                        await cts.CancelAsync();
                        Console.WriteLine("\nBot stopped");
                        break;
                    }

                    Console.WriteLine($"\n{me.Id} - {me.FirstName} - {me.LastName} - {me.Username}");
                }
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
