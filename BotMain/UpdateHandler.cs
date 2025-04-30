using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Threading.Tasks;
using static BotMain.ToDoItem;


namespace BotMain
{
    public class UpdateHandler:IUpdateHandler
    {
        int ver = 1;
        DateOnly date = new DateOnly(2025, 04, 29);
        List<ToDoService> tasks = new List<ToDoService>();
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        public UpdateHandler(IUserService userService, IToDoService toDoService)
        {
            _userService = userService;
            _toDoService = toDoService;
        }

        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            try
                {
                var comand = update.Message.Text.Split(" ")[0]; 
                var textToDo = string.Join(" ", update.Message.Text.Split(" ")[1..]); 

                    if (comand == "/start")
                    {
                        Start(botClient, update);
                    }
                    if (comand == "/help")
                    {
                        Help(botClient, update);
                    }
                    if (comand == "/info")
                    {
                        Info(botClient, update,  ver, date);
                    }
            
                    if (comand == "/addtask")
                    {
                        Addtask(botClient, update, textToDo);
                    }
                    if (comand == "/showtasks")
                    {
                        Showtasks(botClient, update);
                    }
                    if (comand == "/removetask")
                    {
                        Removetask(botClient, update, textToDo);
                    }
                    if (comand == "/completetask")
                    {
                       CompleteTask(botClient, update, textToDo);
                    }
                    if (comand == "/showalltasks")
                    {
                       ShowAlltasks(botClient, update);
                    }
                botClient.SendMessage(update.Message.Chat, "Введите команду");
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
        static void Info(ITelegramBotClient botClient, Update update,  int x, DateOnly d)
        {
            botClient.SendMessage(update.Message.Chat, $"Я {x} версия от {d} бота помошника\n");
           
        }
        //Регистрация пользователя
        private void Start(ITelegramBotClient botClient, Update update)
        {
            _userService.RegisterUser(update.Message.From.Id, update.Message.From.Username!);
            var userNm = _userService.GetUser(update.Message.From.Id);
            botClient.SendMessage(update.Message.Chat,
                $"Добро пожаловать, {userNm.TelegramUserName}!\n" +
                "Введите /help для списка команд");
       
        }
        //справка о командах
        private void Help(ITelegramBotClient botClient, Update update)
        {
            botClient.SendMessage(update.Message.Chat, $"Справка");

            string new_addtask = "/addtask - команда позволяющая добавлять задания\n"+
                                 "/addtask - формат команды: /addtask Имя_задачи\n" +
                                 "/showtasks - команда позволяющая просматривать активные задания пользователя\n" +
                                 "/showalltasks - команда позволяющая просматривать задания пользователя\n" +
                                 "/completetask - команда позволяющая завершать задания\n" +
                                 "/completetask - формат команды: /completetask Id_задачи\n" +
                                 "/removetask - команда позволяющая удалять задания\n"+
                                 "/removetask - формат команды: /completetask Id_задачи\n";

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
            botClient.SendMessage(update.Message.Chat, Help_text);

        }
        //Добавление задачи
        private void Addtask(ITelegramBotClient botClient, Update update, string name)
        {
            var user = _userService.GetUser(update.Message.From.Id);
            if (user == null)
            {
                botClient.SendMessage(update.Message.Chat, $"Команда не доступна");
                return;
            }

            var task_in = _toDoService.Add(user, name);

            botClient.SendMessage(update.Message.Chat, $"Задача  {task_in.Name} добавлена в список\n" +
                                                       $"Id={task_in.Id}");
        }
        //Просмотр активных задач
        private void Showtasks(ITelegramBotClient botClient, Update update)
        {
            if (_userService.GetUser(update.Message.From.Id) == null)
            {
                botClient.SendMessage(update.Message.Chat, $"Команда не доступна");
                return;
            }
            var user = _userService.GetUser(update.Message.From.Id);
            var _tasks = _toDoService.GetActiveByUserId(user.UserId);
            string state = ":";
            if (_tasks.Count == 0)
                state = " пуст";
            string text = $"Ваш список активных задач";

            botClient.SendMessage(update.Message.Chat, $"{text}{state}");


            foreach (var task in _tasks)
            {
                botClient.SendMessage(update.Message.Chat, $"{task.Name}- {task.CreatedAt}- {task.Id}");
            }
        }
        //Просмотр всех задач
        private void ShowAlltasks(ITelegramBotClient botClient, Update update)
        {
            if (_userService.GetUser(update.Message.From.Id) == null)
            {
                botClient.SendMessage(update.Message.Chat, $"Команда не доступна");
                return;
            }
            var user = _userService.GetUser(update.Message.From.Id);
            var _tasks = _toDoService.GetAllByUserId(user.UserId);
            string state = ":";
            if (_tasks.Count == 0)
                state = " пуст";
            string text = $"Ваш список задач";

            botClient.SendMessage(update.Message.Chat, $"{text}{state}");


            foreach (var task in _tasks)
            {
                botClient.SendMessage(update.Message.Chat, $"({task.State})- {task.Name}- {task.CreatedAt}- {task.Id}");
            }
        }
        //Удаление задач
        private void Removetask(ITelegramBotClient botClient, Update update, string id)
        {
            if (_userService.GetUser(update.Message.From.Id) == null)
            {
                botClient.SendMessage(update.Message.Chat, $"Команда не доступна");
                return;
            }
            var user = _userService.GetUser(update.Message.From.Id);
            var _tasks = _toDoService.GetAllByUserId(user.UserId);
            if (_tasks.Count == 0)
            {
                botClient.SendMessage(update.Message.Chat, $"Ваш список задач пуст");
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
                botClient.SendMessage(update.Message.Chat, $"Такой задачи нет");
                return;
            }
            _toDoService.Delete(numberToRemove);

            foreach (var task in _tasks.Where(task => task.Id == numberToRemove))
            {
                botClient.SendMessage(update.Message.Chat, $"Задача {task.Name} удалена");
            }


        }
        //Завершение задач
        private void CompleteTask(ITelegramBotClient botClient, Update update, string id)
        {
            if (_userService.GetUser(update.Message.From.Id) == null)
            {
                botClient.SendMessage(update.Message.Chat, $"Команда не доступна");
                return;
            }
            var user = _userService.GetUser(update.Message.From.Id);
            var _tasks = _toDoService.GetActiveByUserId(user.UserId);
            if (_tasks.Count == 0)
            {
                botClient.SendMessage(update.Message.Chat, $"Ваш список задач пуст");
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
                botClient.SendMessage(update.Message.Chat, $"Такой задачи нет");
                return;
            }
            _toDoService.MarkCompleted(numberToRemove);

            foreach (var task in _tasks.Where(task => task.Id == numberToRemove))
            {
                botClient.SendMessage(update.Message.Chat, $"Задача {task.Name} завершена");
            }

        }

    }
}
