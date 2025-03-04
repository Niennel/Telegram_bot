using System.Text;
using static BotMain.Echo;


namespace BotMain
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding  = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;


            int ver = 1;
            DateOnly date = new DateOnly(2025,02,21);
            string name = "", comand = "";

            Info(ver, date);
                      
                //// Создаем строку с китайскими иероглифами и предпочитаемым шрифтом
                //StyledString chineseString = new StyledString("你好，世界！", "SimSun-ExtB");

                //// Выводим строку в консоль
                //chineseString.PrintToConsole();

                //// Создаем строку с русским текстом и предпочитаемым шрифтом
                //StyledString russianString = new StyledString("Привет, мир!", "Consolas");

                //// Выводим строку в консоль
                //russianString.PrintToConsole();


            //Console.WriteLine(date);

            while (comand != "/exit")
            {
                string b = name == "" ? "В" : ", в";
                Console.WriteLine($"{name}{b}ведите команду");
                comand = Console.ReadLine();

                if (comand == "/start")
                {
                    name=Start(name);
                }
                if (comand == "/help")
                {
                    Help(name);
                }
                if (comand == "/info")
                {
                    Info(name, ver, date);
                }
                if (name!=""&&  comand.Contains( "/echo"))
                {
                    Echo1(comand,name);
                }
            }
        }
        static void Info(int x, DateOnly d)
        {
            Console.WriteLine($"Привет, меня зовут ZiLong!");
            Console.WriteLine("你好，我叫子龙！");
            Console.WriteLine("Сейчас вам доступны следующие команды:");
            Console.WriteLine($"Я {x} версия от {d} бота помощника по изучению китайских слов");
            Console.WriteLine("/start");
            Console.WriteLine("/info");
            Console.WriteLine("/help");
            Console.WriteLine("/exit");
        }
        static void Info(string n , int x, DateOnly d)
        {
            string t =$" {x} версия от {d} бота помощника по изучению китайских слов";
            if (n != "")
                Console.WriteLine($"{n}, я{t}");
            else
                Console.WriteLine($"Я{t}");
        }
        static string Start(string n)
        {

            do
            {
                Console.WriteLine("Как вас зовут?");
                Console.WriteLine("你叫什么名字?");
                n = Console.ReadLine();
            }
            while (n == "");


           Console.WriteLine($"Здравствуйте,{n}");
           Console.WriteLine($"你好，{n}");
            return n;
        }
        static void Help(string n)
        {
            if (n != "")
                Console.WriteLine($"{n}, справка:");
            else
                Console.WriteLine($"Справка");

            Console.WriteLine("Для корректного отображения в консоли");
            Console.WriteLine("1. Требуется языковой пакет Китайский");
            Console.WriteLine("2. Выбери в консоли подходящий шрифт, к примеру NSimSum,KaiTi или SimHei");
            Console.WriteLine("3. Перед завершением лучше переключится на шрифт Consolus");
            Console.WriteLine("/start - команда начала работы, после её ввода программа запросит ваше имя");
            Console.WriteLine("/info - команда для предоставления информации о версии");
            Console.WriteLine("/help - команда вывода справки");
            Console.WriteLine("/exit - команда выхода");
            Console.WriteLine("/echo - команда Эхо, формат записи: /echo слово");

        }

    }
}
