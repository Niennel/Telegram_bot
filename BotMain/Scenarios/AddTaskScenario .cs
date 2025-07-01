using BotMain.Entities;
using BotMain.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotMain.Scenarios
{
    internal class AddTaskScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;

        public AddTaskScenario(IUserService userService, IToDoService toDoService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _toDoService = toDoService ?? throw new ArgumentNullException(nameof(toDoService));
        }

        bool IScenario.CanHandle(ScenarioType scenario) => scenario == ScenarioType.AddTask;

        public async Task<ScenarioResult> HandleMessageAsync(
        ITelegramBotClient botClient,
        ScenarioContext context,
        Update update,
        CancellationToken ct)
        {
            string textToDo = update.Message.Text;
            switch (context.CurrentStep)
            {
                case null:
                    // Шаг 1: Получение пользователя
                    var user = await _userService.GetUser(update.Message.From.Id, ct);
                    if (user == null)
                    {
                        await botClient.SendMessage(
                            update.Message.Chat.Id,
                            "Пользователь не найден. Сначала выполните /start",
                            cancellationToken: ct);
                        return ScenarioResult.Completed;
                    }
                    context.Data["User"] = user;
                    
                    
                    if (textToDo.Contains(" "))
                    { 
                        textToDo = string.Join(" ", update.Message.Text.Split(" ")[1..]);
                        context.Data["Name"] = textToDo;
                        context.CurrentStep = "Deadline";
                          await botClient.SendMessage(
                          update.Message.Chat.Id,
                          "Введите срок выполнения (дд.ММ.гггг) или нажмите /skip:",
                          cancellationToken: ct);
                        return ScenarioResult.Transition;
                    }

                    
                    context.CurrentStep = "Name";

                    await botClient.SendMessage(
                        update.Message.Chat.Id,
                        "Введите название задачи:",
                        cancellationToken: ct);

                    return ScenarioResult.Transition;

                case "Name":
                    // Шаг 2: Сохранение названия и запрос даты
                    
                    context.Data["Name"] = textToDo;
                    context.CurrentStep = "Deadline";

                    await botClient.SendMessage(
                        update.Message.Chat.Id,
                        "Введите срок выполнения (дд.ММ.гггг) или нажмите /skip:",
                        cancellationToken: ct);

                    return ScenarioResult.Transition;

                case "Deadline":
                    // Шаг 3: Обработка даты
                    DateTime? deadline = null;

                    if (update.Message.Text != "/skip")
                    {
                        if (!DateTime.TryParseExact(
                            update.Message.Text,
                            "dd.MM.yyyy",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out var parsedDate))
                        {
                            await botClient.SendMessage(
                                update.Message.Chat.Id,
                                "Неверный формат даты. Введите в формате дд.ММ.гггг:",
                                cancellationToken: ct);
                            return ScenarioResult.Transition;
                        }

                        deadline = parsedDate;
                    }

                    // Создание задачи
                    try
                    {
                        var userFromContext = (ToDoUser)context.Data["User"];
                        var name = (string)context.Data["Name"];
                        var task = await _toDoService.Add(userFromContext, name, deadline, ct);

                        var response = deadline.HasValue
                            ? $"Задача '{task.Name}' добавлена с сроком до {deadline.Value:dd.MM.yyyy}"
                            : $"Задача '{task.Name}' добавлена без срока";

                        await botClient.SendMessage(
                            update.Message.Chat.Id,
                            response,
                            cancellationToken: ct);

                        return ScenarioResult.Completed;
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendMessage(
                            update.Message.Chat.Id,
                            $"Ошибка: {ex.Message}",
                            cancellationToken: ct);
                        return ScenarioResult.Completed;
                    }

                default:
                    await botClient.SendMessage(
                        update.Message.Chat.Id,
                        "Неизвестный шаг сценария",
                        cancellationToken: ct);
                    return ScenarioResult.Completed;
            }
        }
    }
}
