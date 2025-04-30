using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMain
{
    public class ToDoItem (string name, ToDoUser  user)
    {
        public enum ToDoItemState
        {
            Active,
            Completed
        }
      
        public Guid Id { get; set; } = Guid.NewGuid();
        public ToDoUser User { get; set; } = user;
        public string Name { get; set; } = name;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ToDoItemState State { get; set; } = ToDoItemState.Active;
        public DateTime? StateChangedAt { get; set; } = DateTime.UtcNow;

    }
}
