using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                var files = Directory.GetFiles(x.FullName).ToList();
                files.ForEach(f =>
                {
                    Console.WriteLine(f);

                    var lines = File.ReadAllLines(f);

                    using (StreamWriter writer = new StreamWriter(f))
                    {
                        lines.ToList().ForEach(l =>
                        {
                            var libSubs = l.Split('|');
                            var sub = libSubs[1];

                            if(Subs.ContainsKey(sub))
                            {
                                Subs[sub] +=1;
                            }
                            else
                            {
                                Subs[sub] = 1;
                            }

                            if (ContainsUnicodeCharacter(l))
                            {
                                writer.WriteLine(StripUnicodeCharactersFromString(l));
                            }
                            else
                            {
                                writer.WriteLine(l);
                            }
                        });                        
                    }

                    if(f.Contains("-Error"))
                    {
                        File.Move(f, f.Replace("-Error", string.Empty));
                    }                    
                });
            });

            foreach (KeyValuePair<string, int> entry in Subs)
            {
                Console.WriteLine(entry.Key + " -> " + entry.Value);
            }

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
