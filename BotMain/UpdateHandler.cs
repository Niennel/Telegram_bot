using BotMain.Core.DataAccess;
using BotMain.Exceptions;
using BotMain.Services;
//using Otus.ToDoList.ConsoleBot;
//using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static BotMain.Entities.ToDoItem;

namespace BotMain
{
    class UpdateHandler(IUserService _userService, IToDoService _toDoService, IToDoReportService _toDoReportService) : IUpdateHandler
    {
        int ver = 1;
        DateOnly date = new DateOnly(2025, 04, 29);
        //List<ToDoService> tasks = new List<ToDoService>();
        //private readonly IUserService _userService;
        //private readonly IToDoService _toDoService;
        //private readonly IToDoReportService _toDoReportService;
        HandleErrorSource hes = new HandleErrorSource();
        public delegate void MessageEventHandler(string message);

        public event MessageEventHandler? OnHandleUpdateStarted;
        public event MessageEventHandler? OnHandleUpdateCompleted;
      
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
           
            //await GetKeyboardForUser(botClient, update, ct);
            await SetBotCommands(botClient, update, ct);


            try
            {
                
                OnHandleUpdateStarted?.Invoke(update.Message.Text);
                var comand = update.Message.Text.Split(" ")[0];
                var textToDo = string.Join(" ", update.Message.Text.Split(" ")[1..]);

                if (comand == "/start")
                {
                    await Start(botClient, update, ct);
                    await SetBotCommands(botClient, update, ct);
                }
                if (comand == "/help")
                {
                    await Help(botClient, update, ct);
                }
                if (comand == "/info")
                {
                    await Info(botClient, update, ver, date, ct);
                }

                if (comand == "/addtask")
                {
                    await Addtask(botClient, update, textToDo, ct);
                }
                if (comand == "/showtasks")
                {
                    await Showtasks(botClient, update, ct);
                }
                if (comand == "/removetask")
                {
                    await Removetask(botClient, update, textToDo, ct);
                }
                if (comand == "/completetask")
                {
                    await CompleteTask(botClient, update, textToDo, ct);
                }
                if (comand == "/showalltasks")
                {
                    await ShowAlltasks(botClient, update, ct);
                }
                if (comand == "/report")
                {
                    await Report(botClient, update, _toDoReportService, ct);
                }
                if (comand == "/find")
                {
                    await Find(botClient, update, textToDo, ct);
                }
                OnHandleUpdateCompleted?.Invoke(update.Message.Text);
                //await botClient.SendMessage(update.Message.Chat, "Введите команду", ct);
            }
            catch (ArgumentException e)
            {
                await HandleErrorAsync(botClient, e, hes, ct);
                //throw; 
            }
            catch (TaskCountLimitException e)
            {
                await HandleErrorAsync(botClient, e, hes, ct);
                //throw;
            }
            catch (TaskLengthLimitException e)
            {
                await HandleErrorAsync(botClient, e, hes, ct);
                //throw;
            }
            catch (DuplicateTaskException e)
            {
                await HandleErrorAsync(botClient, e, hes, ct);
                //throw;
            }
            catch (Exception e)
            {
                await HandleErrorAsync(botClient, e, hes, ct);
                //throw;
            }
            finally { await botClient.SendMessage(update.Message.Chat, "Введите команду", cancellationToken: ct, 
                        replyMarkup: await GetKeyboardForUser(botClient, update, ct)); }
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
        public async Task Info(ITelegramBotClient botClient, Update update, int x, DateOnly d, CancellationToken cts)
        {
            await botClient.SendMessage(update.Message.Chat, $"Я {x} версия от {d} бота помошника\n", cancellationToken: cts);
        }
        //Регистрация пользователя
        private async Task Start(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            await _userService.RegisterUser(update.Message.From.Id, update.Message.From.Username!, cts);
            var userNm = await _userService.GetUser(update.Message.From.Id, cts);
            // var uName = userNm.TelegramUserName;
            await botClient.SendMessage(update.Message.Chat,
                $"Добро пожаловать, {userNm.TelegramUserName}!\n" +
                "Введите /help для списка команд", cancellationToken: cts);

        }
        //справка о командах
        private async Task Help(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            await botClient.SendMessage(update.Message.Chat, $"Справка", cancellationToken: cts);
            string new_addtask = "/addtask - команда позволяющая добавлять задания\n" +
                                 "/addtask - формат команды: /addtask Имя_задачи\n" +
                                 "/showtasks - команда позволяющая просматривать активные задания пользователя\n" +
                                 "/showalltasks - команда позволяющая просматривать задания пользователя\n" +
                                 "/completetask - команда позволяющая завершать задания\n" +
                                 "/completetask - формат команды: /completetask Id_задачи\n" +
                                 "/removetask - команда позволяющая удалять задания\n" +
                                 "/removetask - формат команды: /completetask Id_задачи\n" +
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

            var user = await _userService.GetUser(update.Message.From.Id, cts);
            if (user != null)
            {
                Help_text = Help_text + new_addtask;
            }
            await botClient.SendMessage(update.Message.Chat, Help_text, cancellationToken: cts);

        }
        //Добавление задачи
        private async Task Addtask(ITelegramBotClient botClient, Update update, string name, CancellationToken cts)
        {
            var user = await _userService.GetUser(update.Message.From.Id, cts);
            if (user == null)
            {
                await botClient.SendMessage(update.Message.Chat, $"Команда не доступна", cancellationToken: cts);
                return;
            }

            var task_in = await _toDoService.Add(user, name, cts);

            await botClient.SendMessage(update.Message.Chat, $"Задача  {task_in.Name} добавлена в список\n" +
                                                      $"Id={task_in.Id}", cancellationToken: cts);
        }
        //Просмотр активных задач
        private async Task Showtasks(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            if (await _userService.GetUser(update.Message.From.Id, cts) == null)
            {
                await botClient.SendMessage(update.Message.Chat, $"Команда не доступна", cancellationToken: cts);
                return;
            }
            var user = await _userService.GetUser(update.Message.From.Id, cts);
            var _tasks = await _toDoService.GetActiveByUserId(user.UserId, cts);
            string state = ":";
            if (_tasks.Count == 0)
                state = " пуст";
            string text = $"Ваш список активных задач";

            await botClient.SendMessage(update.Message.Chat, $"{text}{state}", cancellationToken: cts);

            var response = new StringBuilder();
            foreach (var task in _tasks)
            {
                _ = response.AppendLine($"{task.Name}\\-\\-\\-{FormatDateTime(task.CreatedAt)}\\-\\-\\-`{task.Id}`");//{task.Name}\\- {task.CreatedAt}\\- 
            }

            await botClient.SendMessage(update.Message.Chat, text:response.ToString(), parseMode: ParseMode.MarkdownV2, cancellationToken: cts);
        }
        //Просмотр всех задач
        private async Task ShowAlltasks(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            if (await _userService.GetUser(update.Message.From.Id, cts) == null)
            {
                await botClient.SendMessage(update.Message.Chat, $"Команда не доступна", cancellationToken: cts);
                return;
            }
            var user = await _userService.GetUser(update.Message.From.Id, cts);
            var _tasks = await _toDoService.GetAllByUserId(user.UserId, cts);
            string state = ":";
            if (_tasks.Count == 0)
                state = " пуст";
            string text = $"Ваш список задач";

            await botClient.SendMessage(update.Message.Chat, $"{text}{state}", cancellationToken: cts);

            var response = new StringBuilder();
            foreach (var task in _tasks)
            {
                _ = response.AppendLine($"\\({task.State}\\)\\-\\-\\-{task.Name}\\-\\-\\-{FormatDateTime(task.CreatedAt)}\\-\\-\\-`{task.Id}`");//{task.Name}\\- {task.CreatedAt}\\- 
            }
            await botClient.SendMessage(update.Message.Chat, text: response.ToString(), parseMode: ParseMode.MarkdownV2, cancellationToken: cts);
        }
        //Удаление задач
        private async Task Removetask(ITelegramBotClient botClient, Update update, string id, CancellationToken cts)
        {
            var user = await _userService.GetUser(update.Message.From.Id, cts);
            if (user == null)
            {
                await botClient.SendMessage(update.Message.Chat, $"Команда не доступна", cancellationToken: cts);
                return;
            }

            var _tasks = await _toDoService.GetAllByUserId(user.UserId, cts);
            if (_tasks.Count == 0)
            {
                await botClient.SendMessage(update.Message.Chat, $"Ваш список задач пуст", cancellationToken: cts);
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
                await botClient.SendMessage(update.Message.Chat, $"Такой задачи нет", cancellationToken: cts);
                return;
            }
            await _toDoService.Delete(numberToRemove, cts);

            foreach (var task in _tasks.Where(task => task.Id == numberToRemove))
            {
                await botClient.SendMessage(update.Message.Chat, $"Задача {task.Name} удалена", cancellationToken: cts);
            }


        }
        //Завершение задач
        private async Task CompleteTask(ITelegramBotClient botClient, Update update, string id, CancellationToken cts)
        {
            if (await _userService.GetUser(update.Message.From.Id, cts) == null)
            {
                await botClient.SendMessage(update.Message.Chat, $"Команда не доступна", cancellationToken: cts);
                return;
            }
            var user = await _userService.GetUser(update.Message.From.Id, cts);
            var _tasks = await _toDoService.GetActiveByUserId(user.UserId, cts);
            if (_tasks.Count == 0)
            {
                await botClient.SendMessage(update.Message.Chat, $"Ваш список задач пуст", cancellationToken: cts);
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
                await botClient.SendMessage(update.Message.Chat, $"Такой задачи нет", cancellationToken: cts);
                return;
            }
            await _toDoService.MarkCompleted(numberToRemove, cts);

            foreach (var task in _tasks.Where(task => task.Id == numberToRemove))
            {
                await botClient.SendMessage(update.Message.Chat, $"Задача {task.Name} завершена", cancellationToken: cts);
            }

        }
        private async Task Report(ITelegramBotClient botClient, Update update, IToDoReportService toDoReport, CancellationToken cts)
        {
            //если пользователь не зарегистрирован, то ничего не происходит при вызове
            if (await _userService.GetUser(update.Message.From.Id, cts) == null)
            {
                await botClient.SendMessage(update.Message.Chat, $"Команда не доступна", cancellationToken: cts);
                return;
            }
            var user = await _userService.GetUser(update.Message.From.Id, cts);
            var tuple = await toDoReport.GetUserStats(user!.UserId, cts);
            await botClient.SendMessage(update.Message.Chat,
            $"Статистика по задачам на {tuple.generatedAt}. Всего: {tuple.total}; Завершенных: {tuple.completed}; Активных: {tuple.active};", cancellationToken: cts);
        }
        private async Task Find(ITelegramBotClient botClient, Update update, string pref, CancellationToken cts)
        {
            var user = await _userService.GetUser(update.Message.From.Id, cts);

            if (user == null)
            {
                await botClient.SendMessage(update.Message.Chat, $"Команда не доступна", cancellationToken: cts);
                return;
            }
            var _tasks = await _toDoService.GetAllByUserId(user.UserId, cts);
            if (_tasks.Count == 0)
            {
                await botClient.SendMessage(update.Message.Chat, $"Ваш список задач пуст", cancellationToken: cts);
                return;
            }

            var _findtasks = await _toDoService.Find(user, pref, cts);

            if (_findtasks.Count == 0)
            {
                await botClient.SendMessage(update.Message.Chat, $"По заданному префиксу ничего не найдено", cancellationToken: cts);
                return;
            }

            foreach (var task in _findtasks)
            {
                await botClient.SendMessage(update.Message.Chat, $"{task.Name}- {task.CreatedAt}- {task.Id}", cancellationToken: cts);
            }


        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource hes, CancellationToken ct)
        {
            Console.WriteLine($"HandleError: {exception})");
            await Task.CompletedTask;
        }

        private async Task <bool> IsUserRegistered(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            var user = await _userService.GetUser(update.Message.From.Id, cts);
            if (user == null)
            {
                return false;
            }
            return true;
        }
        private  async Task <ReplyKeyboardMarkup> GetKeyboardForUser(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            var user = await IsUserRegistered(botClient, update, cts);
            if (user == false)
            {
                return new ReplyKeyboardMarkup(new[] { new[] { new KeyboardButton("/start") } })
                {
                    ResizeKeyboard = true
                };
            }

            return new ReplyKeyboardMarkup(new[]
            {
            new[] { new KeyboardButton("/showalltasks"), new KeyboardButton("/showtasks") },
            new[] { new KeyboardButton("/report") }
            })
            {
                ResizeKeyboard = true
            };

        }
        private async Task SetBotCommands(ITelegramBotClient botClient, Update update, CancellationToken cts)
        {
            var user = await IsUserRegistered(botClient, update, cts);

            var commands = new List<BotCommand>
               {
                  new BotCommand
                  {
                      Command = "start",
                      Description = "Начать работу с ботом"
                  },
                   new BotCommand
                  {
                      Command = "help",
                      Description = "Показать список доступных команд"
                  }

               };
            var commandsAdd = new List<BotCommand>
            {
                new BotCommand
                  {
                      Command = "showalltasks",
                      Description = "Показать все задачи"
                  },
                  new BotCommand
                  {
                      Command = "showtasks",
                      Description = "Показать мои задачи"
                  },
                  new BotCommand
                  {
                      Command = "report",
                      Description = "Отправить отчет"
                  }
            };
            if (user == true)
            {
                commands.AddRange(commandsAdd);
            }
            await botClient.SetMyCommands(
                commands: commands,
                scope: null, // Для всех пользователей
                languageCode: null); // По умолчанию
        }
        private static string FormatDateTime(DateTime dateTime)
        {
            string datePart = EscapeMarkdownV2($"{dateTime:dd.MM.yyyy}");
            string timePart = EscapeMarkdownV2($"{dateTime:HH:mm:ss}");
            return $"`{datePart}` `{timePart}`";
        }
        private static string EscapeMarkdownV2(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            char[] specialChars = { '_', '*', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!' };

            var builder = new StringBuilder();
            foreach (char c in text)
            {
                if (specialChars.Contains(c))
                    builder.Append('\\');
                builder.Append(c);
            }

            return builder.ToString();
        }
    }
}

