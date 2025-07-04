﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMain.Scenarios
{
    internal class InMemoryScenarioContextRepository : IScenarioContextRepository
    {
        private readonly Dictionary<long, ScenarioContext> _Scenario = [];
        public Task<ScenarioContext?> GetContext(long userId, CancellationToken ct)
        {
            if (_Scenario.TryGetValue(userId, out var context))
            {
                return Task.FromResult<ScenarioContext?>(context);
            }
            return Task.FromResult<ScenarioContext?>(null);
        }

        public Task ResetContext(long userId, CancellationToken ct)
        {
            if (_Scenario.ContainsKey(userId))
            {
                _Scenario.Remove(userId);
            }
            return Task.CompletedTask;
        }

        public Task SetContext(long userId, ScenarioContext context, CancellationToken ct)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _Scenario[userId] = context;
            return Task.CompletedTask;
        }
    }
}
