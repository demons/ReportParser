using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportParser
{
    class Program
    {
        static void Main(string[] args)
        {
            //Путь к файлу с отчетом
            string path = @"report.txt";

            try
            {
                //Защищенный блок (здесь выполняем опасные операции. К примеру, нет указаного файла.)

                Encoding encoding = Encoding.Default;
                //Читаем все строки из файла
                string[] lines = System.IO.File.ReadAllLines(path, encoding);

                int startIndex = 1;
                int length = lines.Length - startIndex;
                if (startIndex > lines.Length - 1)
                    throw new Exception("Количество строк, в файле меньше, чем указаный начальный индекс.");

                //Создадим массив, в который будем копировать строки из файла
                string[] result = new string[length];

                //Массив откуда копируем;
                //Индекс, в массиве источнике, начиная с которого будет производиться копирование;
                //Массив назначения;
                //Индекс с которого будет начинаться копирование в массив-назначения;
                //Количество строк, которые нужно скопировать
                Array.Copy(lines, startIndex, result, 0, length);

                List<string[]> prms = new List<string[]>();
                //Теперь в нашем массиве result все строки, кроме первой. Дальше обрабатываем каждую строку, разбивая ее по знакам табуляции
                foreach(string line in result)
                {
                    prms.Add(line.Split('\t'));
                }

                //Теперь в переменной prms хранится список списка параметров))
                //  1 - Параметр1, Параметр2, Параметр3
                //  2 - Параметр1, Параметр2, Параметр3
                //  3 - Параметр1, Параметр2, Параметр3

                //Дальше можешь обрабатывать эти параметры уже как тебе нужно
                //Пройдемся по списку prms
                foreach(string[] paramArray in prms)
                {
                    //В paramArray у тебя список параметров в строке. Можешь обращаться к ним так paramArray[index].
                    foreach(var prm in paramArray)
                    {
                        Console.Write(prm + " | ");
                    }
                    Console.WriteLine();
                }
            }
            catch(Exception ex)
            {
                //Если возникла ошибка в блоке try, то управление программой перейдет в этот блок. Корректно обработаем ошибку.
                //Выведем информацию отб ошибке пользователю
                Console.WriteLine(ex.Message);
            }

            

            //Ожидаем ввода (чтобы консоль не закрылась)
            Console.Read();
        }
    }
}
