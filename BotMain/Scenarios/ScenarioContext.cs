using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMain.Scenarios
{
    internal class ScenarioContext
    {
        //public enum ScenarioType
        //{
        //    None,
        //    AddTask
        //}

        public long UserId { get; set; }//Id пользователя в Telegram
        public ScenarioType CurrentScenario { get; set; }
        public string? CurrentStep { get; set; }
        public Dictionary<string, object> Data { get; }

        public ScenarioContext(ScenarioType scenario)
        {
            CurrentScenario = scenario;
            Data = new Dictionary<string, object>();
        }

        public T GetData<T>(string key, CancellationToken ct)
        {
            if (Data.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return default;
        }

        public void SetData(string key, object value, CancellationToken ct)
        {
            Data[key] = value;
        }

    }
}
