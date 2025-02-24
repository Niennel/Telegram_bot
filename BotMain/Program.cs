using System.Text;


namespace BotMain
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.InputEncoding = Encoding.Unicode;
            //Console.OutputEncoding = Encoding.Unicode;


            int ver = 1;
            DateOnly date = new DateOnly(2025,02,21);
            string name = "", comand = "";

            Console.WriteLine($"Привет, меня зовут!");
            Console.WriteLine("你好，我叫子龙！");
            Console.WriteLine(@"Для корректного отображения:");
            Console.WriteLine("1.  派大星");
            Console.WriteLine("2. Выбери в консоли подходящий шрифт, к примеру NSimSum,KaiTi или SimHei");

           
                // Создаем строку с китайскими иероглифами и предпочитаемым шрифтом
                StyledString chineseString = new StyledString("你好，世界！", "SimSun-ExtB");

                // Выводим строку в консоль
                chineseString.PrintToConsole();

                // Создаем строку с русским текстом и предпочитаемым шрифтом
                StyledString russianString = new StyledString("Привет, мир!", "Consolas");

                // Выводим строку в консоль
                russianString.PrintToConsole();
          

            //Console.WriteLine(date);
        }
    }
}
