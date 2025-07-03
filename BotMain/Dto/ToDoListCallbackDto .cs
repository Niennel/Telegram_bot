using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace BotMain.Dto
{
    public class ToDoListCallbackDto: CallbackDto
    {
        public Guid? ToDoListId { get; set; }

        public static new ToDoListCallbackDto FromString(string input)
        {
            var parts = input.Split('|');
            return new ToDoListCallbackDto
            {
                Action = parts.Length > 0 ? parts[0] : null,
                ToDoListId = parts.Length > 1 && Guid.TryParse(parts[1], out var id) ? id : null
            };
        }

        public override string ToString() => $"{base.ToString()}|{ToDoListId}";
    }
}
