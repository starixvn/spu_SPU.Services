using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Console.Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

            File.AppendAllText(filePath, "Executed at " + DateTime.Now + Environment.NewLine);
        }
    }
}
