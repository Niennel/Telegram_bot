using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMain
{
    public class StyledString
    {
        // Поле для хранения строки
        public string Text { get; private set; }

        // Поле для хранения шрифта
        public string Font { get; private set; }

        // Конструктор для инициализации строки и шрифта
        public StyledString(string text, string font)
        {
            Text = text;
            Font = font;
        }
        public void PrintToConsole()
        {
            // Выводим строку
            Console.WriteLine(Text);

            // Проверяем, совпадает ли текущий шрифт с предпочитаемым
            if (!IsFontApplied())
            {
                Console.WriteLine($"Для корректного отображения измените шрифт консоли на {Font}.");
                Console.WriteLine("Инструкция:");
                Console.WriteLine("1. Щелкните правой кнопкой мыши на заголовке окна консоли.");
                Console.WriteLine("2. Выберите 'Свойства'.");
                Console.WriteLine($"3. На вкладке 'Шрифт' выберите {Font}.");
                Console.WriteLine("4. Нажмите 'ОК'.");
            }
        }
        private bool IsFontApplied()
        {
            // В консоли Windows нет API для получения текущего шрифта,
            // поэтому всегда возвращаем false, чтобы показать инструкцию.
            return false;
        }
    }

}
