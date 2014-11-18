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
        private static Encoding encoding = Encoding.Default;
        static void Main(string[] args)
        {
            //Инициализация
            string sourceDir = Directory.GetCurrentDirectory();
            string destDir = Directory.GetCurrentDirectory();            

            try
            {
                //Обрабатываем командную строку
                for (var i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-d")
                    {
                        if (args.Length <= i + 1)
                            throw new Exception("Неверный формат командной строки! Введите -help для помощи");


                        destDir = args[++i];
                        continue;
                    }
                    if (args[i] == "-s")
                    {
                        if (args.Length <= i + 1)
                            throw new Exception("Неверный формат командной строки! Введите -help для помощи");

                        sourceDir = args[++i];
                        continue;
                    }
                    if (args[i] == "-help")
                    {
                        Console.WriteLine("[-s sourceDir] \n \t- sourceDir каталог, в котором будет производиться поиск отчетов");
                        Console.WriteLine("[-d destDir] \n \t- destDir каталог, в который будет производиться запись готовых отчетов");
                        Console.WriteLine("Если параметры не указаны, по умолчанию будет использоваться текущий каталог для поиска и записи отчетов");
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
                return;
            }

            Console.WriteLine("Поиск отчетов будет производиться в директори " + sourceDir + ". Сохранение готовых отчетов в директорию " + destDir + ".");

            //Найдем все отчеты на диске C:, можно любое другое место
            var files = FindAllTextFilesOfFolder(sourceDir);

            //Инициализируем папку для бекапа
            string backupDir = sourceDir + "\\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

            //Создаем папку для backup'а
            Directory.CreateDirectory(backupDir);

            List<string> lines = new List<string>();

            //Обработаем все найденые файлы
            foreach (var file in files)
            {
                //Парсим отчет
                var prms = ParseReport(file);

                //Перекладываем файл в папку для бекапа
                File.Move(file, backupDir + "\\" + Path.GetFileName(file));

                if(prms == null)
                    Console.WriteLine(string.Format("Файл {0} небыл обработан!", file));

                ReportBuilder(destDir, prms, ref lines);
            }

            //var dir = sourceDir + "\\" + DateTime.Now.ToString("yyy-MM-dd HH-mm");
            //Directory.CreateDirectory(dir);
            System.IO.File.WriteAllLines(sourceDir + "\\report.xls", lines, Encoding.Unicode);
            

            //Ожидаем ввода (чтобы консоль не закрылась)
            Console.Read();
        }

        private static IEnumerable<string> FindAllTextFilesOfFolder(string folder)
        {
            var dirs = Directory.GetFiles(folder);

            List<string> files = new List<string>();

            foreach (var file in dirs)
            {
                //Если файл с указаным расширением, то добавляем в наш список
                if (Path.GetExtension(file).Equals(".txt"))
                    files.Add(file);
            }

            return files;
        }

        private static List<string[]> ParseReport(string path)
        {
            try
            {
                //Защищенный блок (здесь выполняем опасные операции. К примеру, нет указаного файла.)

                //Установим кодировку файла отчета

                //Читаем все строки из файла
                string[] lines = System.IO.File.ReadAllLines(path, encoding);

                int startIndex = 1;
                int length = lines.Length - startIndex;
                if (startIndex > lines.Length - 1)
                    throw new Exception("Количество строк, в файле меньше, чем указаный начальный индекс.");

                //Создадим массив, в который будем копировать строки из файла
                string[] resultLines = new string[length];

                //Массив откуда копируем;
                //Индекс, в массиве источнике, начиная с которого будет производиться копирование;
                //Массив назначения;
                //Индекс с которого будет начинаться копирование в массив-назначения;
                //Количество строк, которые нужно скопировать
                Array.Copy(lines, startIndex, resultLines, 0, length);

                List<string[]> prms = new List<string[]>();
                //Теперь в нашем массиве result все строки, кроме первой. Дальше обрабатываем каждую строку, разбивая ее по знакам табуляции
                foreach (string line in resultLines)
                {
                    prms.Add(line.Split('\t'));
                }

                Console.WriteLine(string.Format("Файл {0}, успешно обработан.", path));

                return prms;

                //Теперь в переменной prms хранится список списка параметров))
                //  1 - Параметр1, Параметр2, Параметр3
                //  2 - Параметр1, Параметр2, Параметр3
                //  3 - Параметр1, Параметр2, Параметр3

                //Дальше можешь обрабатывать эти параметры уже как тебе нужно
                //Пройдемся по списку prms
                /*foreach (string[] paramArray in prms)
                {
                    //В paramArray у тебя список параметров в строке. Можешь обращаться к ним так paramArray[index].
                    foreach (var prm in paramArray)
                    {
                        Console.Write(prm + " | ");
                    }
                    Console.WriteLine();
                }*/
            }
            catch (Exception ex)
            {
                //Если возникла ошибка в блоке try, то управление программой перейдет в этот блок. Корректно обработаем ошибку.
                //Выведем информацию отб ошибке пользователю
                Console.WriteLine(ex.Message);

                return null;
            }
        }


        //Создаем отчет
        /// <summary>
        /// Генератор отчета
        /// </summary>
        /// <param name="destPath">Путь для сохранения готового отчета</param>
        /// <param name="parms">Список пропарсеных параметров</param>
        private static void ReportBuilder(string destPath, List<string[]> parms, ref List<string> lines)
        {
            foreach(var line in parms)
            {
                string str = "";
                foreach(var t in line)
                {
                    str += t + "\t";
                }
                lines.Add(str);
            }
        }
    }
}
