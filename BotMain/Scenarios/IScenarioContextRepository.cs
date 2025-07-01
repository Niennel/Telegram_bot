using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMain.Scenarios
{
    //Репозиторий, который отвечает за доступ к контекстам пользователей
    interface IScenarioContextRepository
    {
        //Получить контекст пользователя
        Task<ScenarioContext?> GetContext(long userId, CancellationToken ct);

        //Задать контекст пользователя
        Task SetContext(long userId, ScenarioContext context, CancellationToken ct);

        //Сбросить (очистить) контекст пользователя
        Task ResetContext(long userId, CancellationToken ct);
    }
}
