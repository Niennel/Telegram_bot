using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMain
{
   public class TaskCountLimitException (int taskCountLimit): Exception ($"Превышено максимальное количество задач равное {taskCountLimit}");
    public class TaskLengthLimitException(int taskLength, int taskLengthLimit) : Exception($"Длина задачи '{taskLength}' превышает максимально допустимое значение { taskLengthLimit }");
    public class DuplicateTaskException(string task) : Exception($"Задача '{task}' уже существует");

}
