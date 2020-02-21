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
        static Dictionary<string, int> Subs;
        static void Main(string[] args)
        {
            Console.Write("Input path: ");

            var path = Console.ReadLine();

            var di = new DirectoryInfo(path);
            var subfolders = di.EnumerateDirectories()
                              .OrderBy(d => d.Name)
                              .ToList();
            Subs = new Dictionary<string, int>();

            subfolders.ForEach(x =>
            {
                if (x.FullName.Contains("20200215") || x.FullName.Contains("20200216") || x.FullName.Contains("20200217"))
                {
                    var files = Directory.GetFiles(x.FullName).ToList();

                    files.ForEach(f =>
                    {
                        var lines = File.ReadAllLines(f);

                        var result = new List<string>();

                        lines.ToList().ForEach(l =>
                        {
                            var libSubs = l.Split('|');
                            var sub = libSubs[1];

                            if (!sub.Contains("XTD3") && !sub.Contains("IBO") && !sub.Contains("Alpha789y") && !sub.Contains("MAXBET789y"))
                            {
                                result.Add(l);
                            }
                        });

                        if (result.Count > 0)
                        {
                            File.WriteAllLines(f, result);
                            Console.WriteLine("Re-Writed file => " + f);
                        }
                        else
                        {
                            File.Delete(f);
                            Console.WriteLine("DELETED file => " + f);
                        }
                    });
                }
            });

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
