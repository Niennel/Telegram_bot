using System;
using System.Collections.Generic;


namespace BotMain
{
    internal class Echo
    {
        public static void Echo1(string x, string c)
        {
            if (x.Contains("/echo "))
                Console.WriteLine(x.Replace("/echo ", ""));
            else
                Console.WriteLine($"{c}, ошибка формата");
        }
    }
}
