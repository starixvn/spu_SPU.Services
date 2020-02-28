using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MigrateCorrectCSVFile
{
    class Program
    {        
        static void Main(string[] args)
        {
            Console.Write("Input path: ");
            var path = Console.ReadLine();

            Console.Write("Input Log path: ");
            var logPath = Console.ReadLine();

            var files = Directory.GetFiles(path).ToList();
            var  Subs = new List<string>();

            files.ForEach(f =>
               {
                   if (!Subs.Contains(f))
                   {
                       Console.WriteLine("Sub " + f + " is added.");

                       Subs.Add(f);
                   }
               });

            //var di = new DirectoryInfo(path);
            //var subfolders = di.EnumerateDirectories()
            //                  .OrderBy(d => d.Name)
            //                  .ToList();
            //Subs = new List<string>();

            //subfolders.ForEach(x =>
            //{
            //    var files = Directory.GetFiles(x.FullName).ToList();

            //    files.ForEach(f =>
            //    {
            //        if (!Subs.Contains(f))
            //        {
            //            Console.WriteLine("Sub " + f + " is added.");

            //            Subs.Add(f);
            //        }

            //        //var lines = File.ReadAllLines(f);                                        

            //        //lines.ToList().ForEach(l =>
            //        //{
            //        //    var libSubs = l.Split('|');
            //        //    var sub = libSubs[1];

            //        //    if(!Subs.Contains(sub))
            //        //    {
            //        //        Console.WriteLine("Sub " + sub + " is added.");

            //        //        Subs.Add(sub);
            //        //    }
            //        //});                    
            //    });
            //});

            File.WriteAllLines(logPath + "//subs.log", Subs);
            Console.WriteLine("DONE");
            Console.ReadKey();
        }
        public static bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);
        }

        public static String StripUnicodeCharactersFromString(string inputValue)
        {
            return Regex.Replace(inputValue, @"[^\u0000-\u007F]", String.Empty);
        }
    }
}
