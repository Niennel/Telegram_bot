﻿using System;
using System.Text;
using System.Text.Unicode;
using static BotMain.Echo;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BotMain
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            int ver = 1;
            DateOnly date = new DateOnly(2025,02,21);
            string name = "", comand = "";
            List<string> tasks = new List<string>();

            Info(ver, date);
                      
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
                if (comand == "/addtask")
                {
                    Addtask(name, ref tasks);
                }
                if (comand == "/showtasks")
                {
                    Showtasks(name, tasks);
                }
                if (comand == "/removetask")
                {
                    Removetask(name, ref tasks);
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
            Console.WriteLine("/addtask");
            Console.WriteLine("/showtasks");
            Console.WriteLine("/removetask");
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
        static void Addtask(string n, ref List<string> t)
        {
            string text = $"ведите описание задания";
            string task_in;
            do
            {
                if (n != "")
                    Console.WriteLine($"{n}, в{text}");
                else
                    Console.WriteLine($"В{text}");
                task_in = (Console.ReadLine());
            }
            while (task_in == "");
            t.Add(task_in);
            Console.WriteLine($"Задача \"{task_in}\" добавлена");
        }
        static void Showtasks(string n, List<string> t)
        {
            string state = ":";
            if (t.Count==0)
                state = " пуст";
            string text = $"аш список задач";
            if (n != "")
                Console.WriteLine($"{n}, в{text}{state}");
            else
                Console.WriteLine($"В{text}{state}");
            for (int i = 0; i < t.Count; i++)
            {
                Console.WriteLine($"{i}. {t[i]}");
            }
        }
        static void Removetask(string n, ref List<string> t )
        {
            Showtasks(n, t);
            if (t.Count == 0)
                return;

            string num;
            bool isNumber;
            int number;
            do
            {
                Console.WriteLine("Введите номер строки для удаления");
                num = (Console.ReadLine());
                isNumber = int.TryParse(num, out number);

                if (isNumber)
                {
                    if (t.Count > number)
                    {
                        Console.WriteLine($"Задача \"{t[number]}\" удалена");
                        t.RemoveAt(number);
                        break;
                    }
                    else 
                    {
                        Console.WriteLine("Введенное значение отсутствует в перечне задач.");
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("Введенная строка не является целым числом.");
                }
            }
            while (isNumber == true & t.Count < number | isNumber == false);
            //return (Console.ReadLine());
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
            Console.WriteLine("/addtask - команда позволяющая добавлять задания");
            Console.WriteLine("/showtasks - команда позволяющая просматривать задания");
            Console.WriteLine("/removetask - команда позволяющая удалять задания");

        }

    }
}
