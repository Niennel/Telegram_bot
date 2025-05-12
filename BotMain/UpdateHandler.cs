using BotMain.Core.DataAccess;
using BotMain.Exceptions;
using BotMain.Services;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Threading.Tasks;
using static BotMain.Entities.ToDoItem;


namespace BotMain
{
     class UpdateHandler(IUserService _userService, IToDoService _toDoService, IToDoReportService _toDoReportService, CancellationTokenSource cts) : IUpdateHandler
    {
        int ver = 1;
        DateOnly date = new DateOnly(2025, 04, 29);
        //List<ToDoService> tasks = new List<ToDoService>();
        //private readonly IUserService _userService;
        //private readonly IToDoService _toDoService;
        //private readonly IToDoReportService _toDoReportService;
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            try
                {
                   var comand = update.Message.Text.Split(" ")[0]; 
                   var textToDo = string.Join(" ", update.Message.Text.Split(" ")[1..]); 

                    if (comand == "/start")
                    {
                        Start(botClient, update, ct);
                    }
                    if (comand == "/help")
                    {
                        Help(botClient, update, ct);
                    }
                    if (comand == "/info")
                    {
                        Info(botClient, update,  ver, date,ct);
                    }
            
                    if (comand == "/addtask")
                    {
                        Addtask(botClient, update, textToDo, ct);
                    }
                    if (comand == "/showtasks")
                    {
                        Showtasks(botClient, update, ct);
                    }
                    if (comand == "/removetask")
                    {
                        Removetask(botClient, update, textToDo, ct);
                    }
                    if (comand == "/completetask")
                    {
                       CompleteTask(botClient, update, textToDo, ct);
                    }
                    if (comand == "/showalltasks")
                    {
                        ShowAlltasks(botClient, update, ct);
                    }
                    if (comand == "/report")
                    {
                         Report(botClient, update, _toDoReportService, ct);
                    }
                    if (comand == "/find")
                    {
                        Find(botClient, update, textToDo, ct);
                    }
                   
                  await botClient.SendMessage(update.Message.Chat, "Введите команду", ct);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
                catch (TaskCountLimitException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
                catch (TaskLengthLimitException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
                catch (DuplicateTaskException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Произошла непредвиденная ошибка: ");
                    Console.WriteLine(
                        $"e.GetType = {e.GetType()}\ne.Message = {e.Message}\ne.StackTrace = {e.StackTrace}\ne.InnerException = {e.InnerException}");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
               

        }
        //static void Info(int x, DateOnly d)
        //{
        //    Console.WriteLine($"Привет, меня зовут ZiLong!");
        //    Console.WriteLine("你好，我叫子龙！");
        //    Console.WriteLine("Сейчас вам доступны следующие команды:");
        //    Console.WriteLine($"Я {x} версия от {d} бота помощника по изучению китайских слов");
        //    Console.WriteLine("/start");
        //    Console.WriteLine("/info");
        //    Console.WriteLine("/help");
        //    Console.WriteLine("/addtask");
        //    Console.WriteLine("/showtasks");
        //    Console.WriteLine("/removetask");
        //    Console.WriteLine("/exit");
        //}
        //информаци о версии
        public async Task Info(ITelegramBotClient botClient, Update update,  int x, DateOnly d, CancellationToken cts)
        {
            await botClient.SendMessage(update.Message.Chat, $"Я {x} версия от {d} бота помошника\n",cts);
        }
        //Регистрация пользователя
        private async Task Start(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            _userService.RegisterUser(update.Message.From.Id, update.Message.From.Username!);
            var userNm = _userService.GetUser(update.Message.From.Id);
            await botClient.SendMessage(update.Message.Chat,
                $"Добро пожаловать, {userNm.TelegramUserName}!\n" +
                "Введите /help для списка команд",cts);

        }
        //справка о командах
        private async Task Help(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            await botClient.SendMessage(update.Message.Chat, $"Справка", cts);
            string new_addtask = "/addtask - команда позволяющая добавлять задания\n"+
                                 "/addtask - формат команды: /addtask Имя_задачи\n" +
                                 "/showtasks - команда позволяющая просматривать активные задания пользователя\n" +
                                 "/showalltasks - команда позволяющая просматривать задания пользователя\n" +
                                 "/completetask - команда позволяющая завершать задания\n" +
                                 "/completetask - формат команды: /completetask Id_задачи\n" +
                                 "/removetask - команда позволяющая удалять задания\n"+
                                 "/removetask - формат команды: /completetask Id_задачи\n"+
                                 "/report - команда возвращающая статистику польщователя\n" +
                                 "/find - команда позволяющая искать задания по префиксу\n" +
                                 "/find - кформат команды: /find префикс\n";

              string Help_text = "Для корректного отображения в консоли\n"
                                + "1. Требуется языковой пакет Китайский\n"
                                + "2. Выбери в консоли подходящий шрифт, к примеру NSimSum,KaiTi или SimHei\n"
                                + "3. Перед завершением лучше переключится на шрифт Consolus\n"
                                + "Для выхода нажмите Ctrl+C\n"
                                + "/start - команда начала работы, после её ввода программа запросит ваше имя\n"
                                + "/info - команда для предоставления информации о версии\n"
                                + "/help - команда вывода справки\n";

            if (_userService.GetUser(update.Message.From.Id) != null)
            {
                Help_text = Help_text + new_addtask ;
            }
            await botClient.SendMessage(update.Message.Chat, Help_text, cts);

        }
        //Добавление задачи
        private async Task Addtask(ITelegramBotClient botClient, Update update, string name, CancellationToken cts)
        {
            var user = _userService.GetUser(update.Message.From.Id);
            if (user == null)
            {
                await botClient.SendMessage(update.Message.Chat, $"Команда не доступна", cts);
                return;
            }

            var task_in = _toDoService.Add(user, name);

            await botClient.SendMessage(update.Message.Chat, $"Задача  {task_in.Name} добавлена в список\n" +
                                                      $"Id={task_in.Id}", cts);
        }
        //Просмотр активных задач
        private async Task Showtasks(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            if (_userService.GetUser(update.Message.From.Id) == null)
            {
                await botClient.SendMessage(update.Message.Chat, $"Команда не доступна", cts);
                return;
            }
            var user = _userService.GetUser(update.Message.From.Id);
            var _tasks = _toDoService.GetActiveByUserId(user.UserId);
            string state = ":";
            if (_tasks.Count == 0)
                state = " пуст";
            string text = $"Ваш список активных задач";

            await botClient.SendMessage(update.Message.Chat, $"{text}{state}", cts);

            Console.WriteLine($"Привет, меня зовут ZiLong!");
            foreach (var task in _tasks)
            {
                await botClient.SendMessage(update.Message.Chat, $"{task.Name}- {task.CreatedAt}- {task.Id}", cts);
            }
        }
        //Просмотр всех задач
        private async Task ShowAlltasks(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            if (_userService.GetUser(update.Message.From.Id) == null)
            {
                await botClient.SendMessage(update.Message.Chat, $"Команда не доступна", cts);
                return;
            }
            var user = _userService.GetUser(update.Message.From.Id);
            var _tasks = _toDoService.GetAllByUserId(user.UserId);
            string state = ":";
            if (_tasks.Count == 0)
                state = " пуст";
            string text = $"Ваш список задач";

            await botClient.SendMessage(update.Message.Chat, $"{text}{state}", cts);


            foreach (var task in _tasks)
            {
                await botClient.SendMessage(update.Message.Chat, $"({task.State})- {task.Name}- {task.CreatedAt}- {task.Id}", cts);
            }
        }
        //Удаление задач
        private async Task Removetask(ITelegramBotClient botClient, Update update, string id, CancellationToken cts)
        {
            if (_userService.GetUser(update.Message.From.Id) == null)
            {
                await botClient.SendMessage(update.Message.Chat, $"Команда не доступна", cts);
                return;
            }
            var user = _userService.GetUser(update.Message.From.Id);
            var _tasks = _toDoService.GetAllByUserId(user.UserId);
            if (_tasks.Count == 0)
            {
                await botClient.SendMessage(update.Message.Chat, $"Ваш список задач пуст", cts);
                return;
            }

            Guid numberToRemove;
            var isGuid = false;
            isGuid = Guid.TryParse(id, out numberToRemove);

            var taskToRemove = _tasks
              .Where(task => task.Id == numberToRemove)
              .ToList();
            if (taskToRemove.Count == 0)
            {
                await botClient.SendMessage(update.Message.Chat, $"Такой задачи нет", cts);
                return;
            }
            _toDoService.Delete(numberToRemove);

            foreach (var task in _tasks.Where(task => task.Id == numberToRemove))
            {
                await botClient.SendMessage(update.Message.Chat, $"Задача {task.Name} удалена", cts);
            }


        }
        //Завершение задач
        private async Task CompleteTask(ITelegramBotClient botClient, Update update, string id, CancellationToken cts)
        {
            if (_userService.GetUser(update.Message.From.Id) == null)
            {
                await botClient.SendMessage(update.Message.Chat, $"Команда не доступна", cts);
                return;
            }
            var user = _userService.GetUser(update.Message.From.Id);
            var _tasks = _toDoService.GetActiveByUserId(user.UserId);
            if (_tasks.Count == 0)
            {
                await botClient.SendMessage(update.Message.Chat, $"Ваш список задач пуст", cts);
                return;
            }

            Guid numberToRemove;
            var isGuid = false;
            isGuid = Guid.TryParse(id, out numberToRemove);

            var taskToRemove = _tasks
              .Where(task => task.Id == numberToRemove)
              .ToList();
            if (taskToRemove.Count == 0)
            {
                await botClient.SendMessage(update.Message.Chat, $"Такой задачи нет", cts);
                return;
            }
            _toDoService.MarkCompleted(numberToRemove);

            foreach (var task in _tasks.Where(task => task.Id == numberToRemove))
            {
                await botClient.SendMessage(update.Message.Chat, $"Задача {task.Name} завершена", cts);
            }

        }
        private async Task Report(ITelegramBotClient botClient, Update update, IToDoReportService toDoReport, CancellationToken cts)
        {
            //если пользователь не зарегистрирован, то ничего не происходит при вызове
            if (_userService.GetUser(update.Message.From.Id) == null)
            {
                await botClient.SendMessage(update.Message.Chat, $"Команда не доступна", cts);
                return;
            }

            var tuple = toDoReport.GetUserStats(_userService.GetUser(update.Message.From.Id)!.UserId);
            await botClient.SendMessage(update.Message.Chat,
            $"Статистика по задачам на {tuple.generatedAt}. Всего: {tuple.total}; Завершенных: {tuple.completed}; Активных: {tuple.active};", cts);
        }
        private async Task Find(ITelegramBotClient botClient, Update update, string pref, CancellationToken cts)
        {
            if (_userService.GetUser(update.Message.From.Id) == null)
            {
                await botClient.SendMessage(update.Message.Chat, $"Команда не доступна", cts);
                return;
            }
            var user = _userService.GetUser(update.Message.From.Id);
            var _tasks = _toDoService.GetAllByUserId(user.UserId);
            if (_tasks.Count == 0)
            {
                await botClient.SendMessage(update.Message.Chat, $"Ваш список задач пуст", cts);
                return;
            }

            var _findtasks = _toDoService.Find(user,pref);

            if (_findtasks.Count == 0)
            {
                await botClient.SendMessage(update.Message.Chat, $"По заданному префиксу ничего не найдено", cts);
                return;
            }

            foreach (var task in _findtasks)
            {
                await botClient.SendMessage(update.Message.Chat, $"{task.Name}- {task.CreatedAt}- {task.Id}", cts);
            }


        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
        {
            Console.WriteLine($"HandleError: {exception})");
            return Task.CompletedTask;
        }
    }
}
